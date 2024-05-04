using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MQTTnet.Client;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DateTimePoint = LiveChartsCore.Defaults.DateTimePoint;
using MqttClient = DemoCaseGui.Core.Application.Communication.MqttClient;
using Timer = System.Timers.Timer;

namespace DemoCaseGui.Core.Application.ViewModels
{
   
    public class CaseViewModel : BaseViewModel, INotifyPropertyChanged
    {
        #region Khoi tao
        private readonly S7Client _s7Client;
        private readonly MqttClient _mqttClient;
        private readonly Timer _timer;
        private readonly Timer CheckConnected;
        private Action? NotConnted;

        private CancellationTokenSource _cancellationTokenSource;
        public bool IsConnected => _s7Client.IsConnected;
        public bool IsStatus { get; set; }
        public bool IsON { get; set; }
        public bool IsMqttConnected => _mqttClient.IsConnected;

        private readonly List<DateTimePoint> _values = new();
        private readonly DateTimeAxis _customAxis;

        public bool? ledGreen_old, ledRed_old, ledYellow_old, dCMotor_old, statusIF6123_old, statusKT5112_old, statusO5C500_old, statusUGT524_old;
        public float? angleRB3100_old, tempTW2000_old;
        public ushort? countRB3100_old, distanceUGT524_old, setpoint_old, speed_old;
        //light and DC motor
        public bool? LedGreen { get; set; }
        public bool? LedRed { get; set; }
        public bool? LedYellow { get; set; }
        public bool? DCMotor { get; set; }
        //sensor
        public float? RB3100Angle { get; set; }
        public ushort? RB3100Count { get; set; }
        public float? TW2000Temp { get; set; }
        public bool? IF6123Status { get; set; }
        public bool? KT5112Status { get; set; }
        public bool? O5C500Status { get; set; }
        public bool? UGT524Status { get; set; }
        public ushort? UGT524Distance { get; set; }
        public ushort Resolution { get; set; } = 0;

        //inverter
        public bool? Status { get; set; } = false;
        public bool? Direction { get; set; }
        public bool? ButtonStartup { get; set; }
        public bool? ButtonStop { get; set; }
        public bool? MotorForward { get; set; }
        public bool? MotorReverse { get; set; }
        public ushort MotorSetpointWrite { get; set; }
        public ushort? MotorSetpoint { get; set; }
        public ushort? MotorSpeed { get; set; }

        //Siemens Demo Case 
        public bool? Toggle1 { get; set; }
        public bool? Toggle2 { get; set; }
        public bool? Toggle3 { get; set; }
        public bool? Toggle4 { get; set; }
        public bool? Toggle5 { get; set; }
        public bool? Toggle6 { get; set; }
        public bool? Toggle7 { get; set; }
        public bool? Toggle8 { get; set; }
        public float SetpointWriteSpeed { get; set; } = 0;
        public float SetpointWritePosition { get; set; } = 0;
        //
        public bool? Led0 { get; set; }
        public bool? Led1 { get; set; }
        public bool? Led2 { get; set; }
        public bool? Led3 { get; set; }
        public bool? Led4 { get; set; }
        public bool? Led5 { get; set; }
        public bool? Led6 { get; set; }
        public bool? Led7 { get; set; }

        //STEP MOTOR
        public float? CurrentSpeed { get; set; }
        public float? CurrentPosition { get; set; }
        public float? CurrentPositionReal { get; set; }
        public bool? Mode { get; set; }
        public bool? mode;
        public bool? ModeAuto { get; set; }
        public bool? ModeManual { get; set; }
        public float? SetpointSpeed { get; set; }
        public float? SetpointPosition { get; set; }

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand ResolutionOKCommand { get; set; }
        public ICommand MotorSetpointOKCommand { get; set; }
        public ICommand SpeedOKCommand { get; set; }
        public ICommand PositionOKCommand { get; set; }
        public ICommand StartStepMotorCommand { get; set; }
        public ICommand SethomeStepMotorCommand { get; set; }
        public ICommand AutoModeStepMotorCommand { get; set; }
        public ICommand ManualModeStepMotorCommand { get; set; }
        public ICommand StartInverterCommand { get; set; }
        public ICommand StopInverterCommand { get; set; }

        public ICommand FWDInverter { get; set; }
        public ICommand REVInverter { get; set; }

        public ICommand ResetEncoderCommand { get; set; }
        public ICommand FWDStepMotor { get; set; }
        public ICommand BACKFWDStepMotor { get; set; }


        //Loading animation
        public bool Isbusy { get; set; }
        #endregion Khoi tao

        public  CaseViewModel()
        {
            _s7Client = new S7Client();
            _mqttClient = new MqttClient();
            _timer = new Timer(300);
            _timer.Elapsed += TimerElapsed;
            _mqttClient.ApplicationMessageReceived += OnApplicationMessageReceived;
            ConnectCommand = new RelayCommand(Connect);
            DisconnectCommand = new RelayCommand(Disconnect);
            ResolutionOKCommand = new RelayCommand(WriteResolution);
            MotorSetpointOKCommand = new RelayCommand(WriteMotorSetpoint);
            SpeedOKCommand = new RelayCommand(WriteSpeed);
            PositionOKCommand = new RelayCommand(WritePosition);
            StartStepMotorCommand = new RelayCommand(StartInverterStep);
            SethomeStepMotorCommand = new RelayCommand(SethomeInverterStep);
            AutoModeStepMotorCommand = new RelayCommand(AutoInverterStep);
            StartInverterCommand = new RelayCommand(StratInverter);
            StopInverterCommand = new RelayCommand(StopInverter);
            FWDInverter = new RelayCommand(FWD);
            REVInverter = new RelayCommand(REV);
            ManualModeStepMotorCommand = new RelayCommand(ManualInverterStep);
            FWDStepMotor = new RelayCommand(FWD_StepMotor);
            BACKFWDStepMotor = new RelayCommand(BACKFWD_StepMotor);
            ResetEncoderCommand = new RelayCommand(ResetInverterStep);
            IsON = true;

            CheckConnected = new Timer(300);
            
            CheckConnected.Elapsed += CheckConnected_Elapsed;

            _cancellationTokenSource =  new CancellationTokenSource();
            Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = _values,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };

            _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
            {
                CustomSeparators = GetSeparators(),
                AnimationsSpeed = TimeSpan.FromMilliseconds(0),
                SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
            };

            XAxes = new LiveChartsCore.SkiaSharpView.Axis[] { _customAxis };

            _ = ReadData();

            

        }

        private async void CheckConnected_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(_cancellationTokenSource.IsCancellationRequested )
            {
                _timer.Enabled = false;
                CheckConnected.Stop();
                IsStatus = false;
                IsON = true;
                MessageBox.Show("Mất kết nối với địa chỉ IP: 192.168.1.1 & 192.168.1.11! ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                //CheckConnect
                Task CheckConnect = new(() =>
                {
                    if (!PingHost("192.168.1.1").Result && !PingHost("192.168.1.11").Result)
                    {
                        _cancellationTokenSource.Cancel();
                    }
                });
                CheckConnect.Start();
                await CheckConnect;
            }

        }


        public ObservableCollection<ISeries> Series { get; set; }

        public LiveChartsCore.SkiaSharpView.Axis[] XAxes { get; set; }
        public object Sync { get; } = new object();

        public bool IsReading { get; set; } = true;
        private async Task ReadData()
        {
            // to keep this sample simple, we run the next infinite loop 
            // in a real application you should stop the loop/task when the view is disposed 

            while (IsReading)
            {
                await Task.Delay(100);

                // Because we are updating the chart from a different thread 
                // we need to use a lock to access the chart data. 
                // this is not necessary if your changes are made in the UI thread. 
                lock (Sync)
                {
                    _values.Add(new DateTimePoint(DateTime.Now, MotorSpeed));
                    if (_values.Count > 250) _values.RemoveAt(0);

                    // we need to update the separators every time we add a new point 
                    _customAxis.CustomSeparators = GetSeparators();
                }
            }

        }
        private double[] GetSeparators()
        {
            var now = DateTime.Now;

            return new double[]
            {
            now.AddSeconds(-25).Ticks,
            now.AddSeconds(-20).Ticks,
            now.AddSeconds(-15).Ticks,
            now.AddSeconds(-10).Ticks,
            now.AddSeconds(-5).Ticks,
            now.Ticks
            };
        }

        private static string Formatter(DateTime date)
        {
            var secsAgo = (DateTime.Now - date).TotalSeconds;

            return secsAgo < 1
                ? "now"
                : $"{secsAgo:N0}s ago";
        }
        private async void TimerElapsed(object? sender, EventArgs args)
        {
            if ((bool?)_s7Client.GetTagValue("ledGreen") != ledGreen_old)
            {
                LedGreen = (bool?)_s7Client.GetTagValue("ledGreen");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledGreen", JsonConvert.SerializeObject(_s7Client.GetTag("ledGreen")), true);
            }
            ledGreen_old = (bool?)_s7Client.GetTagValue("ledGreen");

            if ((bool?)_s7Client.GetTagValue("ledRed") != ledRed_old)
            {
                LedRed = (bool?)_s7Client.GetTagValue("ledRed");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledRed", JsonConvert.SerializeObject(_s7Client.GetTag("ledRed")), true);
            }
            ledRed_old = (bool?)_s7Client.GetTagValue("ledRed");

            if ((bool?)_s7Client.GetTagValue("ledYellow") != ledYellow_old)
            {
                LedYellow = (bool?)_s7Client.GetTagValue("ledYellow");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledYellow", JsonConvert.SerializeObject(_s7Client.GetTag("ledYellow")), true);
            }
            ledYellow_old = (bool?)_s7Client.GetTagValue("ledYellow");

            if ((bool?)_s7Client.GetTagValue("DCMotor") != dCMotor_old)
            {
                DCMotor = (bool?)_s7Client.GetTagValue("DCMotor");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/DCMotor", JsonConvert.SerializeObject(_s7Client.GetTag("DCMotor")), true);
            }
            dCMotor_old = (bool?)_s7Client.GetTagValue("DCMotor");


            //sensor
            if ((bool?)_s7Client.GetTagValue("statusIF6123") != statusIF6123_old)
            {
                IF6123Status = (bool?)_s7Client.GetTagValue("statusIF6123");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusIF6123", JsonConvert.SerializeObject(_s7Client.GetTag("statusIF6123")), true);
            }
            statusIF6123_old = (bool?)_s7Client.GetTagValue("statusIF6123");

            if ((bool?)_s7Client.GetTagValue("statusKT5112") != statusKT5112_old)
            {
                KT5112Status = (bool?)_s7Client.GetTagValue("statusKT5112");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusKT5112", JsonConvert.SerializeObject(_s7Client.GetTag("statusKT5112")), true);
            }
            statusKT5112_old = (bool?)_s7Client.GetTagValue("statusKT5112");

            if ((bool?)_s7Client.GetTagValue("statusO5C500") != statusO5C500_old)
            {
                O5C500Status = (bool?)_s7Client.GetTagValue("statusO5C500");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusO5C500", JsonConvert.SerializeObject(_s7Client.GetTag("statusO5C500")), true);
            }
            statusO5C500_old = (bool?)_s7Client.GetTagValue("statusO5C500");

            if ((bool?)_s7Client.GetTagValue("statusUGT524") != statusUGT524_old)
            {
                UGT524Status = (bool?)_s7Client.GetTagValue("statusUGT524");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusUGT524", JsonConvert.SerializeObject(_s7Client.GetTag("statusUGT524")), true);
            }
            statusUGT524_old = (bool?)_s7Client.GetTagValue("statusUGT524");

            if ((float?)_s7Client.GetTagValue("angleRB3100") != angleRB3100_old)
            {
                RB3100Angle = (float?)_s7Client.GetTagValue("angleRB3100");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/angleRB3100", JsonConvert.SerializeObject(_s7Client.GetTag("angleRB3100")), true);
            }
            angleRB3100_old = (float?)_s7Client.GetTagValue("angleRB3100");

            if ((ushort?)_s7Client.GetTagValue("countRB3100") != countRB3100_old)
            {
                RB3100Count = (ushort?)_s7Client.GetTagValue("countRB3100");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/countRB3100", JsonConvert.SerializeObject(_s7Client.GetTag("countRB3100")), true);
            }
            countRB3100_old = (ushort?)_s7Client.GetTagValue("countRB3100");

            if ((float?)_s7Client.GetTagValue("tempTW2000") != tempTW2000_old)
            {
                TW2000Temp = (float?)_s7Client.GetTagValue("tempTW2000");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/tempTW2000", JsonConvert.SerializeObject(_s7Client.GetTag("tempTW2000")), true);
            }
            tempTW2000_old = (float?)_s7Client.GetTagValue("tempTW2000");

            if ((ushort?)_s7Client.GetTagValue("distanceUGT524") != distanceUGT524_old)
            {
                if ((ushort?)_s7Client.GetTagValue("distanceUGT524") > 200) UGT524Distance = null;
                else UGT524Distance = (ushort?)_s7Client.GetTagValue("distanceUGT524");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/distanceUGT524", JsonConvert.SerializeObject(_s7Client.GetTag("distanceUGT524")), true);
            }
            distanceUGT524_old = (ushort?)_s7Client.GetTagValue("distanceUGT524");

            //inverter
            ButtonStartup = Status;
            ButtonStop = !Status;
            MotorForward = Direction;
            MotorReverse = !Direction;
            MotorSetpoint = (ushort?)_s7Client.GetTagValue("setpoint");
            if ((ushort?)_s7Client.GetTagValue("speed") != speed_old)
            {
                MotorSpeed = (ushort?)_s7Client.GetTagValue("speed");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/speed", JsonConvert.SerializeObject(_s7Client.GetTag("speed")), true);
            }
            speed_old = (ushort?)_s7Client.GetTagValue("speed");
            //Siemens Demo Case
            Task ReadData = new(() =>
            {
                Toggle1 = (bool?)_s7Client.GetTagValue("toggle1");
                Toggle2 = (bool?)_s7Client.GetTagValue("toggle2");
                Toggle3 = (bool?)_s7Client.GetTagValue("toggle3");
                Toggle4 = (bool?)_s7Client.GetTagValue("toggle4");
                Toggle5 = (bool?)_s7Client.GetTagValue("toggle5");
                Toggle6 = (bool?)_s7Client.GetTagValue("toggle6");
                Toggle7 = (bool?)_s7Client.GetTagValue("toggle7");
                Toggle8 = (bool?)_s7Client.GetTagValue("toggle8");

                Led0 = (bool?)_s7Client.GetTagValue("led0");
                Led1 = (bool?)_s7Client.GetTagValue("led1");
                Led2 = (bool?)_s7Client.GetTagValue("led2");
                Led3 = (bool?)_s7Client.GetTagValue("led3");
                Led4 = (bool?)_s7Client.GetTagValue("led4");
                Led5 = (bool?)_s7Client.GetTagValue("led5");
                Led6 = (bool?)_s7Client.GetTagValue("led6");
                Led7 = (bool?)_s7Client.GetTagValue("led7");

                //Step Motor
                CurrentSpeed = (float?)_s7Client.GetTagValue("current_speed_M");
                CurrentPosition = (float?)_s7Client.GetTagValue("current_position_M");
                CurrentPositionReal = CurrentPosition / 10;
                SetpointSpeed = (float?)_s7Client.GetTagValueI("setpoint_speed_M");
                SetpointPosition = (float?)_s7Client.GetTagValueI("setpoint_position_M");

                Status = (bool?)_s7Client.GetTagValue("statusInverter");
                if (Status == false) Direction = null;
                else
                {
                    if ((bool?)_s7Client.GetTagValue("directionForward") == true)
                        Direction = true;
                    if ((bool?)_s7Client.GetTagValue("directionReverse") == true)
                        Direction = false;
                }
            });

            ReadData.Start();
            await ReadData;
            
        }

      

        public Task<bool> PingHost(string nameOrAddress)
#pragma warning restore CA1822 // Mark members as static
        {
            bool pingable = false;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Ping pinger = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return Task.FromResult(pingable);
        }

        private Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payloadMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var commandMessage = JsonConvert.DeserializeObject<CommandMessage>(payloadMessage);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            switch (commandMessage.name)
            {
                case "controlRed":
                    _s7Client.WritePLC("DB2.DBX16.2", commandMessage.value);
                    break;
                case "controlGreen":
                    _s7Client.WritePLC("DB2.DBX16.0", commandMessage.value);
                    break;
                case "controlYellow":
                    _s7Client.WritePLC("DB2.DBX16.3", commandMessage.value);
                    break;
                case "controlDCMotor":
                    _s7Client.WritePLC("DB2.DBX16.1", commandMessage.value);
                    break;
                //
                case "VFD_Speed_SP":
                    _s7Client.WriteNumberPLC("DB4.DBW8", commandMessage.value);
                    break;
                case "VFD_Start":
                    Status = true;
                    _s7Client.WritePLC("DB4.DBX6.0", commandMessage.value);
                    break;
                case "VFD_Stop":
                    Status = false;
                    Direction = null;
                    _s7Client.WritePLC("DB4.DBX6.1", commandMessage.value);
                    break;
                case "VFD_Forward":
                    if (Status == true) Direction = true;
                    _s7Client.WritePLC("DB4.DBX6.2", commandMessage.value);
                    break;
                case "VFD_Reverse":
                    if (Status == true) Direction = false;
                    _s7Client.WritePLC("DB4.DBX6.3", commandMessage.value);
                    break;
                default:
                    _s7Client.WritePLC(_s7Client.GetTagAddress(commandMessage.name), commandMessage.value);
                    break;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return Task.CompletedTask;
        }

        public async void Connect()
        {
            Isbusy = true;            
            await _s7Client.Connect();
            if (IsConnected)
            {
                CheckConnected.Start();
                _timer.Enabled = true;
                IsON = false;
                IsStatus = true;
            }
            else
            {
                _timer.Enabled = false;
                IsStatus = false;
                IsON = true;
            }
       
            await _mqttClient.ConnectAsync();
            //await PublicMqtt();

            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlRed");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlGreen");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlYellow");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlDCMotor");

            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/VFD_Start");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/VFD_Stop");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/VFD_Forward");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/VFD_Reverse");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/VFD_Speed_SP");
            Isbusy = false;

        }
        private void Disconnect()
        {         
            _timer.Enabled = false;
            IsStatus = false;
            IsON = true;
        }

      
        
        public async void WriteResolution()
        {
            Task write = new(() =>
            {
                _s7Client.WriteNumberPLC(_s7Client.GetTagAddress("resolutionRB3100"), Resolution);

            });

            write.Start();
            await write;
        }
        public async void WriteMotorSetpoint()
        {
            Task write = new(() =>
            {
                _s7Client.WriteNumberPLC(_s7Client.GetTagAddress("setpoint"), MotorSetpointWrite);
            });

            write.Start();
            await write;
        }
        public async void WriteSpeed()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB1000.DBD42", SetpointWriteSpeed);
            });

            write.Start();
            await write;
        }

        public async void WritePosition()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB1000.DBD54", SetpointWritePosition);
            });

            write.Start();
            await write;
        }

        public async void StartInverterStep()
        {
            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.2", true);
                Thread.Sleep(1000);
                _s7Client.WritePLCI("DB2.DBX38.2", false);
            });

            write.Start();
            await write;
        }
        public async void ResetInverterStep()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.1", true);
                Thread.Sleep(1000);
                _s7Client.WritePLCI("DB2.DBX38.1", false);
            });

            write.Start();
            await write;
        }
        public async void AutoInverterStep()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.0", false);
                ModeAuto = true;
                ModeManual = false;
            });

            write.Start();
            await write;
        }
        public async void ManualInverterStep()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.0", true);
                ModeAuto = false;
                ModeManual = true;
            });

            write.Start();
            await write;
        }
        public async void SethomeInverterStep()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.5", true);
                Thread.Sleep(1000);
                _s7Client.WritePLCI("DB2.DBX38.5", false);
            });

            write.Start();
            await write;
        }
        public async void StratInverter()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLC("DB4.DBX6.0", true);
                Thread.Sleep(1000);
                _s7Client.WritePLC("DB4.DBX6.0", false);
            });

            write.Start();
            await write;
        }
        public async void StopInverter()
        {
            Task write = new(() =>
            {
                _s7Client.WritePLC("DB4.DBX6.1", true);
                Thread.Sleep(1000);
                _s7Client.WritePLC("DB4.DBX6.1", false);
            });

            write.Start();
            await write;
        }

        public async void FWD()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLC("DB4.DBX6.2", true);
                Thread.Sleep(500);
                _s7Client.WritePLC("DB4.DBX6.2", false);
            });

            write.Start();
            await write;
        }

        public async  void REV()
        {
            //_s7Client.WritePLC("DB4.DBX6.3", true);
            //Thread.Sleep(1000);
            //_s7Client.WritePLC("DB4.DBX6.3", false);

            Task write = new(() =>
            {
                _s7Client.WritePLC("DB4.DBX6.3", true);
                Thread.Sleep(500);
                _s7Client.WritePLC("DB4.DBX6.3", false);
            });

            write.Start();
            await write;
        }

        public async  void FWD_StepMotor()
        {
            //_s7Client.WritePLCI("DB2.DBX38.3", true);
            //Thread.Sleep(1000);
            //_s7Client.WritePLCI("DB2.DBX38.3", false);

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.3", true);
                Thread.Sleep(500);
                _s7Client.WritePLCI("DB2.DBX38.3", false);
            });

            write.Start();
            await write;
        }

        public async void BACKFWD_StepMotor()
        {

            Task write = new(() =>
            {
                _s7Client.WritePLCI("DB2.DBX38.4", true);
                Thread.Sleep(100);
                _s7Client.WritePLCI("DB2.DBX38.4", false);
            });

            write.Start();
            await write;
        }
    }

}
