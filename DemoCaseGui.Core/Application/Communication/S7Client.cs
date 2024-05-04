using HslCommunication;
using HslCommunication.Profinet.Siemens;
using S7.Net;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using Timer = System.Timers.Timer;

namespace DemoCaseGui.Core.Application.Communication;

public class S7Client
{
    private readonly Plc _plc;

    private readonly Plc _plcI;
    

    private readonly Timer _timer;
    public List<Tag> Tags { get; private set; }
    public List<Tag> TagStepMotors { get; private set; }
    public List<MqttTag> MqttTags { get; private set; }

    public bool IsConnected { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public S7Client()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _plc = new Plc(CpuType.S71200, "192.168.1.1", 0, 1);
        _plcI = new Plc(CpuType.S71200, "192.168.1.11", 0, 1);
        _timer = new Timer(300);
        _timer.Elapsed += TimerElapsed;
        Tags = new()
        {
            //Vali_IFM
            new("angleRB3100", "PLC.Vali_IFM.angleRB3100", null, "DB2.DBD2",  DateTime.Now),
            new("countRB3100", "PLC.Vali_IFM.countRB3100", null, "DB2.DBW6",  DateTime.Now),
            new("controlDCMotor", "PLC.Vali_IFM.controlDCMotor", null, "DB2.DBX16.1",  DateTime.Now),
            new("controlGreen", "PLC.Vali_IFM.controlGreen" ,null, "DB2.DBX16.0", DateTime.Now),
            new("controlRed", "PLC.Vali_IFM.controlRed", null, "DB2.DBX16.2",  DateTime.Now),
            new("controlYellow", "PLC.Vali_IFM.controlYellow", null, "DB2.DBX16.3",  DateTime.Now),
            new("DCMotor", "PLC.Vali_IFM.DCmotor", null, "DB2.DBX0.7",  DateTime.Now),
            new("distanceUGT524", "PLC.Vali_IFM.distanceUGT524", null, "DB2.DBW12", DateTime.Now),
            new("ledGreen", "PLC.Vali_IFM.ledGreen", null, "DB2.DBX0.4", DateTime.Now),
            new("ledRed", "PLC.Vali_IFM.ledRed", null, "DB2.DBX0.5",  DateTime.Now),
            new("ledYellow", "PLC.Vali_IFM.ledYellow", null, "DB2.DBX0.6",  DateTime.Now),
            new("statusIF6123", "PLC.Vali_IFM.statusIF6123", null, "DB2.DBX14.1", DateTime.Now),
            new("statusKT5112", "PLC.Vali_IFM.statusKT5112", null, "DB2.DBX14.2", DateTime.Now),
            new("statusO5C500", "PLC.Vali_IFM.statusO5C500", null, "DB2.DBX14.3", DateTime.Now),
            new("statusUGT524", "PLC.Vali_IFM.statusUGT524", null, "DB2.DBX14.0", DateTime.Now),
            new("tempTW2000", "PLC.Vali_IFM.tempTW2000", null, "DB2.DBD8",  DateTime.Now),
            
    
            //inverter 
            new("VFD_Forward", "PLC.Inverter.VFD_Forward", null, "DB4.DBX6.2", DateTime.Now),
            new("VFD_Reverse", "PLC.Inverter.VFD_Reverse", null, "DB4.DBX6.3", DateTime.Now),
            new("statusInverter", "PLC.Inverter.VFD_Run", null, "DB4.DBX10.0", DateTime.Now),
            new("speed", "PLC.Inverter.VFD_Speed_PV", null, "DB4.DBW2", DateTime.Now),
            new("setpoint", "PLC.Inverter.VFD_Speed_SP", null, "DB4.DBW8", DateTime.Now),
            new("VFD_Start", "PLC.Inverter.VFD_Start", null, "DB4.DBX6.0", DateTime.Now),
            new("directionForward", "PLC.Inverter.VFD_Status_Forward", null, "DB4.DBX10.1", DateTime.Now),
            new("directionReverse", "PLC.Inverter.VFD_Status_Reverse", null, "DB4.DBX10.2", DateTime.Now),
            new("VFD_Stop", "PLC.Inverter.VFD_Stop", null, "DB4.DBX6.1", DateTime.Now),
            
            


            //new("mode_M", "PLC.Vali_Siemens.mode", null, "DB8.DBX0.0", DateTime.Now),
            //new("reset_M", "PLC.Vali_Siemens.reset", null, "DB8.DBX0.1", DateTime.Now),
            //new("start_M", "PLC.Vali_Siemens.start", null, "DB8.DBX0.2", DateTime.Now),
            //new("forward_M", "PLC.Vali_Siemens.forward", null, "DB8.DBX0.3", DateTime.Now),
            //new("backward_M", "PLC.Vali_Siemens.backward", null, "DB8.DBX0.4", DateTime.Now),
            //new("home_M", "PLC.Vali_Siemens.setHome", null, "DB8.DBX0.5", DateTime.Now),
           

            


            //Vali_Siemens
            new("led0", "PLC.Vali_Siemens.led1", null, "DB8.DBX2.0", DateTime.Now),
            new("led1", "PLC.Vali_Siemens.led2", null, "DB8.DBX2.1", DateTime.Now),
            new("led2", "PLC.Vali_Siemens.led3", null, "DB8.DBX2.2", DateTime.Now),
            new("led3", "PLC.Vali_Siemens.led4", null, "DB8.DBX2.3", DateTime.Now),
            new("led4", "PLC.Vali_Siemens.led5", null, "DB8.DBX2.4", DateTime.Now),
            new("led5", "PLC.Vali_Siemens.led6", null, "DB8.DBX2.5", DateTime.Now),
            new("led6", "PLC.Vali_Siemens.led7", null, "DB8.DBX2.6", DateTime.Now),
            new("led7", "PLC.Vali_Siemens.led8", null, "DB8.DBX2.7", DateTime.Now),
            new("current_position_M", "PLC.Vali_Siemens.Position_PV", null, "DB8.DBD16", DateTime.Now),
           
            new("resolutionRB3100", "PLC.Vali_IFM.RB3100_reSLT", null, "DB8.DBW20", DateTime.Now),
            
            new("current_speed_M", "PLC.Vali_Siemens.Speed_PV", null, "DB8.DBD12", DateTime.Now),
            new("toggle1", "PLC.Vali_Siemens.toggle1", null, "DB8.DBX0.0", DateTime.Now),
            new("toggle2", "PLC.Vali_Siemens.toggle2", null, "DB8.DBX0.1", DateTime.Now),
            new("toggle3", "PLC.Vali_Siemens.toggle3", null, "DB8.DBX0.2", DateTime.Now),
            new("toggle4", "PLC.Vali_Siemens.toggle4", null, "DB8.DBX0.3", DateTime.Now),
            new("toggle5", "PLC.Vali_Siemens.toggle5", null, "DB8.DBX0.4", DateTime.Now),
            new("toggle6", "PLC.Vali_Siemens.toggle6", null, "DB8.DBX0.5", DateTime.Now),
            new("toggle7", "PLC.Vali_Siemens.toggle7", null, "DB8.DBX0.6", DateTime.Now),
            new("toggle8", "PLC.Vali_Siemens.toggle8", null, "DB8.DBX0.7", DateTime.Now),
        };
        TagStepMotors = new()
        {
              //Step Motor
            new("auto/man", "PLC.Step_Motor.Auto/Man", null, "DB2.DBX2.0", DateTime.Now),
            new("foward", "PLC.Step_Motor.Forward", null, "DB2.DBX38.3", DateTime.Now),
            new("position", "PLC.Step_Motor.Position_Int", null, "DB2.DBW40", DateTime.Now),
            new("reset_encoder", "PLC.Step_Motor.Reset_Encoder", null, "DB2.DBX38.1", DateTime.Now),
            new("reverse", "PLC.Step_Motor.Reverse", null, "DB2.DBX38.4", DateTime.Now),
            new("sethome", "PLC.Step_Motor.SetHome", null, "DB2.DBX38.5", DateTime.Now),
            new("setpoint_speed_M", "PLC.Vali_Siemens.Speed_SP", null, "DB1000.DBD42", DateTime.Now),
             new("setpoint_position_M", "PLC.Vali_Siemens.Position_SP", null, "DB1000.DBD54", DateTime.Now),
            new("start", "PLC.Step_Motor.Start", null, "DB2.DBX38.2", DateTime.Now),
        };

    }

    public async Task Connect()
    {      
        Task connect = new(() =>
        {
            try
            {
                _plc.Open();
                _plcI.Open();
                _timer.Enabled = true;
                IsConnected = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        });

        connect.Start();
        await connect;
    }

    

    public void Disconnect()
    {
        _timer.Enabled = false;
        _plc.Close();
        _plcI.Close();
    }
    public async void TimerElapsed(object? sender, EventArgs args)
    {   
        Task[] readingTasks = Tags.Select(tag => ReadTag(tag)).Concat(TagStepMotors.Select(tag => ReadTagStepMotor(tag))).ToArray();
        await Task.WhenAll(readingTasks);

    }
    
    public async Task ReadTag(Tag tag)
    {
        
        try
        {
            var  value = await _plc.ReadAsync(tag.address);
            if (tag.name is "angleRB3100" or "tempTW2000" or "current_position_M" or "current_speed_M" or "setpoint_position_M" or "setpoint_speed_M")
            {
                tag.value = S7.Net.Conversion.ConvertToFloat(Convert.ToUInt32(value));
            }
            else
            {
                tag.value = value;
            }
            MqttTags = Tags.Select(e => new MqttTag(
                    e.name,
                    e.value,
                    e.timestamp)).ToList();
        }
        catch 
        {
            _timer.Enabled = false;
        }
        
    }

    public async Task ReadTagStepMotor(Tag tag)
    {

        try
        {
            var value = await _plcI.ReadAsync(tag.address);
            if (tag.name is  "setpoint_position_M" or "setpoint_speed_M")
            {
                tag.value = S7.Net.Conversion.ConvertToFloat(Convert.ToUInt32(value));
            }
            else
            {
                tag.value = value;
            }
            MqttTags = Tags.Select(e => new MqttTag(
                    e.name,
                    e.value,
                    e.timestamp)).ToList();
        }
        catch
        {
            _timer.Enabled = false;
        }

    }
    public async void WritePLC(string address, object value)
    {
        await _plc.WriteAsync(address, value);
    }

    public async void WritePLCI(string address, object value)
    {
        await _plcI.WriteAsync(address, value);
    }

    public async void WriteNumberPLC(string address, object value)
    {
        var number = Convert.ToUInt16(value);
        await _plc.WriteAsync(address, number);
    }

    public object? GetTagValue(string tagName)
    {
        return Tags.First(x => x.name == tagName).value;
    }
    public object? GetTagValueI(string tagName)
    {
        return TagStepMotors.First(x => x.name == tagName).value;
    }
    public string GetTagAddress(string tagName)
    {
        return Tags.First(x => x.name == tagName).address;
    }

    public MqttTag GetTag(string tagName)
    {
        return MqttTags.Find(x => x.name == tagName);

    }
    //var mqtttag = Tags.Select(x => new MqttTag(x.name, x.value, x.timestamp));
    //    return (MqttTag) mqtttag;
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
    

}