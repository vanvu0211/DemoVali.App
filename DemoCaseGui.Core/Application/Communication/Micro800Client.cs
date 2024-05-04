using DemoCaseGui.Core.Application.ViewModels;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using LiveChartsCore.Kernel;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Timer = System.Timers.Timer;

namespace DemoCaseGui.Core.Application.Communication
{
    public class Micro800Client: BaseViewModel
    {
        private readonly AllenBradleyMicroCip plc;
        private readonly AllenBradleyMicroCip plc2;
        private readonly Timer _timer;
        public List<Tag> Tags { get; private set; }
        public List<Tag> Tags2 { get; private set; }

        public bool IsConnected { get; set; }



        public Micro800Client()
        {
            //Khoi tao PLC
            plc = new AllenBradleyMicroCip("192.168.1.50");
            plc2 = new AllenBradleyMicroCip("192.168.1.20");
            _timer = new Timer(300);
            _timer.Elapsed += _timer_Elapsed;
            Tags = new()
        {
            
            //TrafficLights
        
            new("led2", "Micro.Micro850.Traffic_RedLightA", null, "_IO_EM_DO_02", DateTime.Now),
            new("led3", "Micro.Micro850.Traffic_YellowLightA", null, "_IO_EM_DO_03", DateTime.Now),
            new("led4", "Micro.Micro850.Traffic_GreenLightA", null, "_IO_EM_DO_04", DateTime.Now),
            new("led5", "Micro.Micro850.Traffic_RedLightB", null, "_IO_EM_DO_05", DateTime.Now),
            new("led6", "Micro.Micro850.Traffic_YellowLightB", null, "_IO_EM_DO_00", DateTime.Now),
            new("led7", "Micro.Micro850.Traffic_GreenLightB", null, "_IO_EM_DO_01", DateTime.Now),

            new("edit_redled", "Micro.Micro850.Traffic_RedTime", null, "HMI_DB.Traffic_Lights.Red_Time", DateTime.Now),
            new("edit_yellowled", "Micro.Micro850.Traffic_YellowTime", null, "HMI_DB.Traffic_Lights.Yellow_Time", DateTime.Now),
            new("edit_greenled", "Micro.Micro850.Traffic_GreenTime", null, "HMI_DB.Traffic_Lights.Green_Time", DateTime.Now),

            new("time_display_a", "Micro.Micro850.Traffic_Display_A", null, "HMI_DB.Traffic_Lights.Time_Display_A", DateTime.Now),
            new("time_display_b", "Micro.Micro850.Traffic_Display_B", null, "HMI_DB.Traffic_Lights.Time_Display_B", DateTime.Now),


            new("confirm_trafficlight", "Micro.Micro850.Traffic_Confirm", null, "HMI_DB.Traffic_Lights.Confirm", DateTime.Now),
            new("start_trafficlight", "Micro.Micro850.Traffic_Start", null, "HMI_DB.Traffic_Lights.Start", DateTime.Now),
            new("stop_trafficlight", "Micro.Micro850.Traffic_Stop", null, "HMI_DB.Traffic_Lights.Stop", DateTime.Now),

            //Inverter
            new("start_inverter", "Micro.Micro850.Inverter_Start", null, "HMI_DB.Inverter.Start", DateTime.Now),
            new("stop_inverter", "Micro.Micro850.Inverter_Stop", null, "HMI_DB.Inverter.Stop", DateTime.Now),
            new("setpoint", "Micro.Micro850.Inverter_Speed_SP", null, "HMI_DB.Inverter.Speed_SP", DateTime.Now),
            new("speed", "Micro.Micro850.Inverter_Speed_PV", null, "HMI_DB.Inverter.Speed_PV", DateTime.Now),
            new("forward", "Micro.Micro850.Inverter_Fwd", null, "HMI_DB.Inverter.Fwd", DateTime.Now),
            new("reverse", "Micro.Micro850.Inverter_Rev", null, "HMI_DB.Inverter.Rev", DateTime.Now),
            new("confirm_inverter", "Micro.Micro850.Inverter_Confirm", null, "HMI_DB.Inverter.Confirm", DateTime.Now),

            new("inverter_active", "Micro.Micro850.Inverter_Active", null, "INVERTER_ACTIVE", DateTime.Now),
            new("inverter_ready", "Micro.Micro850.Inverter_Ready", null, "INVERTER_READY", DateTime.Now),
            new("inverter_error", "Micro.Micro850.Inverter_Error", null, "INVERTER_ERROR", DateTime.Now),
            new("inverter_fwd_status", "Micro.Micro850.Inverter_Fwd_Status", null, "INVERTER_FWD_STATUS", DateTime.Now),
            new("inverter_rev_status", "Micro.Micro850.Inverter_Rev_Status", null, "INVERTER_REV_STATUS", DateTime.Now),
        };
            Tags2 = new()
        {
            new("i0.0", "Micro.Micro820.Micro820_input_1", null, "_IO_EM_DI_04", DateTime.Now),
            new("i0.1", "Micro.Micro820.Micro820_input_2", null, "_IO_EM_DI_05", DateTime.Now),
            new("i0.2", "Micro.Micro820.Micro820_input_3", null, "_IO_EM_DI_06", DateTime.Now),
            new("i0.3", "Micro.Micro820.Micro820_input_4", null, "_IO_EM_DI_07", DateTime.Now),
            new("i0.4", "Micro.Micro820.Micro820_input_5", null, "_IO_EM_DI_08", DateTime.Now),
            new("i0.5", "Micro.Micro820.Micro820_input_6", null, "_IO_EM_DI_09", DateTime.Now),
            new("i0.6", "Micro.Micro820.Micro820_input_7", null, "_IO_EM_DI_10", DateTime.Now),
            new("i0.7", "Micro.Micro820.Micro820_input_8", null, "_IO_EM_DI_11", DateTime.Now),
            new("analog", "Micro.Micro820.Micro820_Analog_1", null, "_IO_EM_AI_00", DateTime.Now),

        };
        }

        private  async void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Task[] readingTasks = Tags.Select(tag => ReadTagMicro850(tag)).Concat(Tags2.Select(tag => ReadTagMicro820(tag))).ToArray();
            await Task.WhenAll(readingTasks);        

        }
       
        public async Task ReadTagMicro850(Tag tag)
        {

            if (tag.name is "speed")
            {
                OperateResult<float> data = await plc.ReadFloatAsync(tag.address);

                if (data.IsSuccess)
                {
                    // you get the right value
                    tag.value = data.Content;
                    //tag.value = rd.NextDouble();
                }
                else
                {
                    

                }

            }
            else if (tag.name is "edit_redled" or "edit_yellowled" or "edit_greenled")
            {
                OperateResult<UInt16> data =await plc.ReadUInt16Async(tag.address);

                if (data.IsSuccess)
                {
                    // you get the right value
                    tag.value = data.Content;
                }
                else
                {
                    // failed , but you still can know the failed detail

                }
            }
            else if (tag.name is "setpoint" or "time_display_a" or "time_display_b")
            {
                OperateResult<Byte> data = await plc.ReadByteAsync(tag.address);

                if (data.IsSuccess)
                {
                    // you get the right value
                    object value = data.Content;
                    tag.value = Convert.ToUInt16(value);
                }
                else
                {
                    // failed , but you still can know the failed detail

                }
            }
            else
            {
                OperateResult<bool> data = plc.ReadBool(tag.address);

                if (data.IsSuccess)
                {
                    // you get the right value
                    tag.value = data.Content;
                }
                else
                {
                    // failed , but you still can know the failed detail

                }
            }
        }
        public async Task ReadTagMicro820(Tag tag)
        {
            if (tag.name is "analog")
            {
                OperateResult<UInt16> data = await plc2.ReadUInt16Async(tag.address);

                if (data.IsSuccess)
                {
                    // you get the right value
                    tag.value = data.Content;
                }
                else
                {
                    // failed , but you still can know the failed detail

                }
            }

            else
            {
                OperateResult<bool> data = await plc2.ReadBoolAsync(tag.address);

                if (data.IsSuccess)
                {
                    // you get the right value
                    tag.value = data.Content;
                }
                else
                {
                    // failed , but you still can know the failed detail

                }
            }
        }
        public object? GetTagValue(string tagName)
        {
            return Tags.First(x => x.name == tagName).value;

        }

        public object? GetTagValue2(string tagName)
        {
            return Tags2.First(x => x.name == tagName).value;

        }

        public string GetTagAddress(string tagName)
        {
            return Tags.First(x => x.name == tagName).address;
        }

        public string GetTagAddress2(string tagName)
        {
            return Tags2.First(x => x.name == tagName).address;
        }

        public void WritePLC(string TagName, bool value)
        {
          
            OperateResult write1 =  plc.Write(TagName, value);
          
        }
        public void WriteNumberPLC(string TagName, UInt16 value)
        {
            OperateResult write =  plc.Write(TagName,value);      
        }


        public async Task Connect()
        {
            Task connect = new( () =>
            {
                try
                {
                    OperateResult connect =  plc.ConnectServer();
                    OperateResult connect2 =  plc2.ConnectServer();
                    if (connect.IsSuccess && connect2.IsSuccess)
                    {
                        _timer.Enabled = true;
                        IsConnected = true;
                    }
                    else
                    {
                        MessageBox.Show("Request Timeout", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            });
            connect.Start();
            await connect;
                           
        }
        public void Disconnect()
        {
            plc.ConnectCloseAsync();
            plc2.ConnectCloseAsync();
            _timer.Enabled = false;
        }



#pragma warning disable CA1822 // Mark members as static
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
}
