using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using LiveCharts.Defaults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using DateTimePoint = LiveChartsCore.Defaults.DateTimePoint;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Net.NetworkInformation;


namespace DemoCaseGui.Core.Application.ViewModels
{
    public class CaseCompactLogixViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly CPLogixClient _CPLogixClient;
        private readonly Timer CheckConnected;
        private readonly Timer _timer;

        private readonly List<DateTimePoint> _values = new();
        private readonly DateTimeAxis _customAxis;
        public bool IsConnected => _CPLogixClient.IsConnected;
        public bool Status { get; set; }
        public bool IsON { get; set; }
        private CancellationTokenSource _cancellationTokenSource;




        //Status Inverter
        public bool? Active { get; set; }
        public bool? Ready { get; set; }
        public bool? Fwd { get; set; }
        public bool? Rev { get; set; }
        public bool? Error { get; set; }
        //IO
        public bool? I0_0 { get; set; }
        public bool? I0_1 { get; set; }
        public bool? I0_2 { get; set; }
        public bool? I0_3 { get; set; }

        public bool? Q0_0 { get; set; }
        public bool? Q0_1 { get; set; }
        public bool? Q0_2 { get; set; }
        public bool? Q0_3 { get; set; }

        public bool? i0_0, i0_1, i0_2, i0_3, q0_0, q0_1, q0_2, q0_3;
        public bool? Start_auto, Start_manual, start_auto_old, start_manual_old, Stop_auto, Stop_manual, stop_auto_old, stop_manual_old, Start_Inverter, start_inverter_old, Stop_Inverter, stop_inverter_old;
        public bool? LED_AUTO_ON { get; set; }
        public bool? LED_MANUAL_ON { get; set; }

        public bool? LED_AUTO_OFF { get; set; }
        public bool? LED_MANUAL_OFF { get; set; }

        public bool? LED_INVERTER_ON { get; set; }
        public bool? LED_INVERTER_OFF { get; set; }
        //Traffic Lights
        public bool? DO1 { get; set; }
        public bool? DO2 { get; set; }
        public bool? XANH1 { get; set; }
        public bool? XANH2 { get; set; }
        public bool? VANG1 { get; set; }
        public bool? VANG2 { get; set; }

        public bool? do1, do2, xanh1, xanh2, vang1, vang2;

        public ushort? SET_D1 { get; set; }
        public ushort? SET_V1 { get; set; }
        public ushort? SET_X1 { get; set; }
        public ushort? set_d1_old, set_v1_old, set_x1_old;

        public ushort? Display_D1 { get; set; }
        public ushort? Display_V1 { get; set; }
        public ushort? Display_X1 { get; set; }
        public ushort? display_d1_old, display_v1_old, display_x1_old;
        public ushort? Display_D2 { get; set; }
        public ushort? Display_V2 { get; set; }
        public ushort? Display_X2 { get; set; }
        public ushort? display_d2_old, display_v2_old, display_x2_old;

        public ushort? Display_1 { get; set; }
        public ushort? Display_2 { get; set; }

        //AUTO MODE
        public ushort? TIME_DO1_AUTO { get; set; }
        public ushort? TIME_DO2_AUTO { get; set; }
        public ushort? TIME_XANH1_AUTO { get; set; }
        public ushort? TIME_XANH2_AUTO { get; set; }
        public ushort? TIME_VANG1_AUTO { get; set; }
        public ushort? TIME_VANG2_AUTO { get; set; }

        public ushort? time_do1_auto, time_do2_auto, time_xanh1_auto, time_xanh2_auto, time_vang1_auto, time_vang2_auto;

        //MANUAL MODE
        public ushort? TIME_DO1_MANUAL { get; set; }
        public ushort? TIME_DO2_MANUAL { get; set; }
        public ushort? TIME_XANH1_MANUAL { get; set; }
        public ushort? TIME_XANH2_MANUAL { get; set; }
        public ushort? TIME_VANG1_MANUAL { get; set; }
        public ushort? TIME_VANG2_MANUAL { get; set; }

        public ushort? time_do1_manual, time_do2_manual, time_xanh1_manual, time_xanh2_manual, time_vang1_manual, time_vang2_manual;

        //SENSOR
        public ushort? DEVICE_UGT_524 { get; set; }
        public ushort? DEVICE_KI6000 { get; set; }
        public ushort? DEVICE_O5D_150 { get; set; }
        public ushort? DEVICE_RPV_510 { get; set; }

        public bool? DEVICE_UGT_524_Status { get; set; }
        public bool? DEVICE_KI6000_Status { get; set; }
        public bool? DEVICE_O5D_150_Status { get; set; }
        public bool? DEVICE_RPV_510_Status { get; set; }
        public bool? DEVICE_IGS_232_Status { get; set; }
        public bool? DEVICE_OGT_500_Status { get; set; }

        public ushort? device_ugt_524, device_ki6000, device_o5d_150, device_rpv_510;
        public bool? device_ugt_524_status_old, device_ki6000_status_old, device_o5d_150_status_old, device_rpv_510_status_old, device_igs_232_status_old, device_ogt_500_status_old;

        //Lights IFM
        public bool? DEN_DO_IFM { get; set; }
        public bool? DEN_VANG_IFM { get; set; }
        public bool? DEN_XANH_IFM { get; set; }

        public bool? den_do_ifm, den_vang_ifm, den_xanh_ifm;

        //Inverter
        public float? Speed { get; set; }

        public float? speed_old, motorsetpoint_old;
        public float? Motorsetpoint { get; set; }
        public int MotorsetpointWrite { get; set; }
        public bool? Motor_Status { get; set; }
        public bool? motor_status_old;
        public bool? Direction_Status { get; set; }
        public bool? direction_status_old;

        public bool? Direction { get; set; }
        public bool? ButtonStartup { get; set; }
        public bool? ButtonStop { get; set; }
        public bool? MotorForward { get; set; }
        public bool? MotorReverse { get; set; }
        //Loading Animation
        public bool Isbusy { get; set; }
        //COmmand
        public ICommand ConnectCommand { get; set; }
        public ICommand CheckConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand Start_Auto_Command { get; set; }
        public ICommand Stop_Auto_Command { get; set; }
        public ICommand Start_Manual_Command { get; set; }
        public ICommand Stop_Manual_Command { get; set; }
        public ICommand Start_Inverter_Command { get; set; }
        public ICommand Stop_Inverter_Command { get; set; }
        public ICommand Forward_Inverter_Command { get; set; }
        public ICommand Write_Setpoint_Command { get; set; }
        public ICommand Write_TimeTrafficLights_Command { get; set; }
        public ICommand LoadViewCommand { get; set; }



        public CaseCompactLogixViewModel()
        {
            _CPLogixClient = new CPLogixClient();
            _timer = new Timer(300);
            _timer.Elapsed += _timer_Elapsed;
            Status = false;
            //CheckConnectCommand = new RelayCommand(CheckConnect);
            ConnectCommand = new RelayCommand(Connect);
            DisconnectCommand = new RelayCommand(Disconnect);
            Start_Manual_Command = (new RelayCommand(Manual_Start));
            Stop_Manual_Command = new RelayCommand(Manual_Stop);
            Start_Inverter_Command = new RelayCommand(Inverter_Start);
            Stop_Inverter_Command = new RelayCommand(Inverter_Stop);
            Forward_Inverter_Command = new RelayCommand(Inverter_Forward);
            Write_Setpoint_Command = new RelayCommand(WriteSetpoint);
            Write_TimeTrafficLights_Command = new RelayCommand(Set_TrafficLights);

            CheckConnected = new Timer(300);
            CheckConnected.Start();
            CheckConnected.Elapsed += CheckConnected_Elapsed;
            _cancellationTokenSource = new CancellationTokenSource();
            IsON = true;

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
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                CheckConnected.Stop();
                _timer.Enabled = false;
                Status = false;
                IsON = true;
                MessageBox.Show("Mất kết nối với địa chỉ IP! ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                //CheckConnect
                Task CheckConnect = new(() =>
                {
                    if (!PingHost("192.168.1.101").Result)
                    {
                        _cancellationTokenSource.Cancel();
                    }
                });
                CheckConnect.Start();
                await CheckConnect;
            }
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


        private async void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

            Task ReadData = new Task(() =>
            {


                if ((bool?)_CPLogixClient.GetTagValue("led_do1") != do1)
                {
                    DO1 = (bool?)_CPLogixClient.GetTagValue("led_do1");
                }
                do1 = (bool?)_CPLogixClient.GetTagValue("led_do1");

                if ((bool?)_CPLogixClient.GetTagValue("led_do2") != do2)
                {
                    DO2 = (bool?)_CPLogixClient.GetTagValue("led_do2");
                }
                do2 = (bool?)_CPLogixClient.GetTagValue("led_do2");

                if ((bool?)_CPLogixClient.GetTagValue("led_xanh1") != xanh1)
                {
                    XANH1 = (bool?)_CPLogixClient.GetTagValue("led_xanh1");
                }
                xanh1 = (bool?)_CPLogixClient.GetTagValue("led_xanh1");

                if ((bool?)_CPLogixClient.GetTagValue("led_xanh2") != xanh2)
                {
                    XANH2 = (bool?)_CPLogixClient.GetTagValue("led_xanh2");
                }
                xanh2 = (bool?)_CPLogixClient.GetTagValue("led_xanh2");

                if ((bool?)_CPLogixClient.GetTagValue("led_vang1") != vang1)
                {
                    VANG1 = (bool?)_CPLogixClient.GetTagValue("led_vang1");
                }
                vang1 = (bool?)_CPLogixClient.GetTagValue("led_vang1");

                if ((bool?)_CPLogixClient.GetTagValue("led_vang2") != vang2)
                {
                    VANG2 = (bool?)_CPLogixClient.GetTagValue("led_vang2");
                }
                vang2 = (bool?)_CPLogixClient.GetTagValue("led_vang2");


                if ((ushort?)_CPLogixClient.GetTagValue("set_do1") != set_d1_old)
                {
                    SET_D1 = (ushort?)_CPLogixClient.GetTagValue("set_do1");
                }
                set_d1_old = (ushort?)_CPLogixClient.GetTagValue("set_do1");


                if ((ushort?)_CPLogixClient.GetTagValue("set_xanh1") != set_x1_old)
                {
                    SET_X1 = (ushort?)_CPLogixClient.GetTagValue("set_xanh1");
                }
                set_x1_old = (ushort?)_CPLogixClient.GetTagValue("set_xanh1");


                if ((ushort?)_CPLogixClient.GetTagValue("set_vang1") != set_v1_old)
                {
                    SET_V1 = (ushort?)_CPLogixClient.GetTagValue("set_vang1");
                }
                set_v1_old = (ushort?)_CPLogixClient.GetTagValue("set_vang1");

                if ((ushort?)_CPLogixClient.GetTagValue("time_do1_dp") != display_d1_old)
                {
                    Display_D1 = (ushort?)_CPLogixClient.GetTagValue("time_do1_dp");
                }
                display_d1_old = (ushort?)_CPLogixClient.GetTagValue("time_do1_dp");

                if ((ushort?)_CPLogixClient.GetTagValue("time_do2_dp") != display_d2_old)
                {
                    Display_D2 = (ushort?)_CPLogixClient.GetTagValue("time_do2_dp");
                }
                display_d2_old = (ushort?)_CPLogixClient.GetTagValue("time_do2_dp");

                if ((ushort?)_CPLogixClient.GetTagValue("time_vang1_dp") != display_v1_old)
                {
                    Display_V1 = (ushort?)_CPLogixClient.GetTagValue("time_vang1_dp");
                }
                display_v1_old = (ushort?)_CPLogixClient.GetTagValue("time_vang1_dp");

                if ((ushort?)_CPLogixClient.GetTagValue("time_vang2_dp") != display_v2_old)
                {
                    Display_V2 = (ushort?)_CPLogixClient.GetTagValue("time_vang2_dp");
                }
                display_v2_old = (ushort?)_CPLogixClient.GetTagValue("time_vang2_dp");


                if ((ushort?)_CPLogixClient.GetTagValue("time_xanh1_dp") != display_x1_old)
                {
                    Display_X1 = (ushort?)_CPLogixClient.GetTagValue("time_xanh1_dp");
                }
                display_x1_old = (ushort?)_CPLogixClient.GetTagValue("time_xanh1_dp");

                if ((ushort?)_CPLogixClient.GetTagValue("time_xanh2_dp") != display_x2_old)
                {
                    Display_X2 = (ushort?)_CPLogixClient.GetTagValue("time_xanh2_dp");
                }
                display_x2_old = (ushort?)_CPLogixClient.GetTagValue("time_xanh2_dp");

                if (DO1 == true) Display_1 = Display_D1;
                if (DO2 == true) Display_2 = Display_D2;
                if (VANG1 == true) Display_1 = Display_V1;
                if (VANG2 == true) Display_2 = Display_V2;
                if (XANH1 == true) Display_1 = Display_X1;
                if (XANH2 == true) Display_1 = Display_X2;

                //SENSOR

                if ((ushort?)_CPLogixClient.GetTagValue("ugt_524") != device_ugt_524)
                {
                    DEVICE_UGT_524 = (ushort?)_CPLogixClient.GetTagValue("ugt_524");
                }
                device_ugt_524 = (ushort?)_CPLogixClient.GetTagValue("ugt_524");

                if ((bool?)_CPLogixClient.GetTagValue("ugt_524_st") != device_ugt_524_status_old)
                {
                    DEVICE_UGT_524_Status = (bool?)_CPLogixClient.GetTagValue("ugt_524_st");
                }
                device_ugt_524_status_old = (bool?)_CPLogixClient.GetTagValue("ugt_524_st");

                if ((ushort?)_CPLogixClient.GetTagValue("ki6000") != device_ki6000)
                {
                    DEVICE_KI6000 = (ushort?)_CPLogixClient.GetTagValue("ki6000");
                }
                device_ki6000 = (ushort?)_CPLogixClient.GetTagValue("ki6000");


                if ((bool?)_CPLogixClient.GetTagValue("ki6000_st") != device_ki6000_status_old)
                {
                    DEVICE_KI6000_Status = (bool?)_CPLogixClient.GetTagValue("ki6000_st");
                }
                device_ki6000_status_old = (bool?)_CPLogixClient.GetTagValue("ki6000_st");


                if ((ushort?)_CPLogixClient.GetTagValue("05d_150") != device_o5d_150)
                {
                    DEVICE_O5D_150 = (ushort?)_CPLogixClient.GetTagValue("05d_150");
                }
                device_o5d_150 = (ushort?)_CPLogixClient.GetTagValue("05d_150");

                if ((bool?)_CPLogixClient.GetTagValue("05d_150_st") != device_o5d_150_status_old)
                {
                    DEVICE_O5D_150_Status = (bool?)_CPLogixClient.GetTagValue("05d_150_st");
                }
                device_o5d_150_status_old = (bool?)_CPLogixClient.GetTagValue("05d_150_st");

                if ((ushort?)_CPLogixClient.GetTagValue("rpv_510") != device_rpv_510)
                {
                    DEVICE_RPV_510 = (ushort?)_CPLogixClient.GetTagValue("rpv_510");
                }
                device_rpv_510 = (ushort?)_CPLogixClient.GetTagValue("rpv_510");

                if ((bool?)_CPLogixClient.GetTagValue("rpv_510_st") != device_rpv_510_status_old)
                {
                    DEVICE_RPV_510_Status = (bool?)_CPLogixClient.GetTagValue("rpv_510_st");
                }
                device_rpv_510_status_old = (bool?)_CPLogixClient.GetTagValue("rpv_510_st");

                if ((bool?)_CPLogixClient.GetTagValue("igs_232") != device_igs_232_status_old)
                {
                    DEVICE_IGS_232_Status = (bool?)_CPLogixClient.GetTagValue("igs_232");
                }
                device_igs_232_status_old = (bool?)_CPLogixClient.GetTagValue("igs_232");

                if ((bool?)_CPLogixClient.GetTagValue("ogt_500") != device_ogt_500_status_old)
                {
                    DEVICE_OGT_500_Status = (bool?)_CPLogixClient.GetTagValue("ogt_500");
                }
                device_ogt_500_status_old = (bool?)_CPLogixClient.GetTagValue("ogt_500");

                //Lights IFM

                if ((bool?)_CPLogixClient.GetTagValue("Alarm_den_do_IFM") != den_do_ifm)
                {
                    DEN_DO_IFM = (bool?)_CPLogixClient.GetTagValue("Alarm_den_do_IFM");
                }
                den_do_ifm = (bool?)_CPLogixClient.GetTagValue("Alarm_den_do_IFM");


                if ((bool?)_CPLogixClient.GetTagValue("Alarm_den_vang_IFM") != den_vang_ifm)
                {
                    DEN_VANG_IFM = (bool?)_CPLogixClient.GetTagValue("Alarm_den_vang_IFM");
                }
                den_vang_ifm = (bool?)_CPLogixClient.GetTagValue("Alarm_den_vang_IFM");


                if ((bool?)_CPLogixClient.GetTagValue("Alarm_den_xanh_IFM") != den_xanh_ifm)
                {
                    DEN_XANH_IFM = (bool?)_CPLogixClient.GetTagValue("Alarm_den_xanh_IFM");
                }
                den_xanh_ifm = (bool?)_CPLogixClient.GetTagValue("Alarm_den_xanh_IFM");

                //Inverter
                ButtonStartup = Motor_Status;
                ButtonStop = !Motor_Status;
                MotorForward = Direction;
                MotorReverse = !Direction;


                if ((ushort?)_CPLogixClient.GetTagValue("speed") * 60 != speed_old)
                {
                    Speed = (ushort?)_CPLogixClient.GetTagValue("speed") * 60;
                }
                speed_old = (ushort?)_CPLogixClient.GetTagValue("speed") * 60;


                if ((ushort?)_CPLogixClient.GetTagValue("motor_sp") * 60 != motorsetpoint_old)
                {
                    Motorsetpoint = (ushort?)_CPLogixClient.GetTagValue("motor_sp") * 60;
                }
                motorsetpoint_old = (ushort?)_CPLogixClient.GetTagValue("motor_sp") * 60;

                if ((bool?)_CPLogixClient.GetTagValue("direction_status_inverter") != direction_status_old)
                {
                    Direction_Status = (bool?)_CPLogixClient.GetTagValue("direction_status_inverter");
                }
                direction_status_old = (bool?)_CPLogixClient.GetTagValue("direction_status_inverter");




                Motor_Status = (bool?)_CPLogixClient.GetTagValue("status_inverter");

                if (Motor_Status == false) Direction = null;
                else
                {
                    if ((bool?)_CPLogixClient.GetTagValue("direction_status_inverter") == true)
                        Direction = true;
                    else
                        Direction = false;
                }

                //Status Inverter
                Active = (bool?)_CPLogixClient.GetTagValue("status_inverter");
                Ready = (bool?)_CPLogixClient.GetTagValue("motor_ready");
                Fwd = (bool?)_CPLogixClient.GetTagValue("direction_status_inverter");
                Rev = !Fwd;
                Error = (bool?)_CPLogixClient.GetTagValue("motor_error");
            });
            ReadData.Start();
            await ReadData;

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
                    _values.Add(new DateTimePoint(DateTime.Now,Speed));
                    if (_values.Count > 250) _values.RemoveAt(0);

                    // we need to update the separators every time we add a new point 
                    _customAxis.CustomSeparators = GetSeparators();
                }
            }
        }
#pragma warning disable CA1822 // Mark members as static
        private double[] GetSeparators()
#pragma warning restore CA1822 // Mark members as static
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
      
        public async void Connect()
        {
            Isbusy = true;
            await _CPLogixClient.Connect();
            if (IsConnected)
            {             
                _timer.Enabled = true;
                Status = true;
                IsON = false;
            }
            else
            {
                _timer.Enabled = false;
                Status = false;
                IsON = true;
            }
            Isbusy = false;

        }
        public void Disconnect()
        {
            _CPLogixClient.Disconnect();
            _timer.Enabled = false;
            Status = false;
            IsON = true;
        }

        public async void Manual_Start()
        {
            
            Task write = new(() =>
            {
                _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("start_manual"), true);
                Thread.Sleep(500);
                _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("start_manual"), false);
            });
            write.Start();
            await write;
        }

        public async void Manual_Stop()
        {
            Task write = new(() =>
            {
                _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("stop_manual"), true);
                Thread.Sleep(500);
                _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("stop_manual"), false);
            });
            write.Start();
            await write;
        }


        public async void Inverter_Start()
        {
            Task write = new(() =>
            {
                _CPLogixClient.WritePLC("START_INVERTER_WEB", true);
                Thread.Sleep(1000);
                _CPLogixClient.WritePLC("START_INVERTER_WEB", false);
            });

            write.Start();
            await write;
        }

        public async void Inverter_Stop()
        {
            Task write = new(() =>
            {
                _CPLogixClient.WritePLC("STOP_INVERTER_WEB", true);
                Thread.Sleep(1000);
                _CPLogixClient.WritePLC("STOP_INVERTER_WEB", false);
            });

            write.Start();
            await write;
        }

        public async void Inverter_Forward()
        {
            Task write = new(() =>
            {
                _CPLogixClient.WritePLC("DAOCHIEU_WEB", true);
                Thread.Sleep(1000);
                _CPLogixClient.WritePLC("DAOCHIEU_WEB", false);
            });
            write.Start();
            await write;
        }

        public async void WriteSetpoint()
        {
            
            Task write = new(() =>
            {
                _CPLogixClient.WriteNumberPLC("SPEED_INVERTER", MotorsetpointWrite/60);
            });
            write.Start();
            await write;

        }
        public async void Set_TrafficLights()
        {
            Task write = new(() =>
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                _CPLogixClient.WriteNumberPLC("SET_D1", (int)SET_D1);
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning disable CS8629 // Nullable value type may be null.
                _CPLogixClient.WriteNumberPLC("SET_X1", (int)SET_X1);
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning disable CS8629 // Nullable value type may be null.
                _CPLogixClient.WriteNumberPLC("SET_V1", (int)SET_V1);
#pragma warning restore CS8629 // Nullable value type may be null.
            });
            write.Start();
            await write;
        }

    }
}
