using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Bluetooth;

namespace GoProPilot.Services.Windows;

public class Camera : ICamera
{
    private static readonly Guid NotifyCommandGUID = new("b5f90073-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid NotifyQueryRespGUID = new("b5f90077-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid NotifySettingsGUID = new("b5f90075-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid ReadAPNameGUID = new("b5f90002-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid ReadAPPassGUID = new("b5f90003-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid SendCommandGUID = new("b5f90072-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid SendQueriesGUID = new("b5f90076-aa8d-11e3-9046-0002a5d5c51b");
    private static readonly Guid SetSettingsGUID = new("b5f90074-aa8d-11e3-9046-0002a5d5c51b");
    private readonly BluetoothDevice _device;
    private GattCharacteristic? _notifyCmds = null;
    private GattCharacteristic? _notifyQueryResp = null;
    private GattCharacteristic? _notifySettings = null;
    private GattCharacteristic? _readAPName = null;
    private GattCharacteristic? _readAPPass = null;
    private GattCharacteristic? _sendCmds = null;
    private GattCharacteristic? _sendQueries = null;
    private GattCharacteristic? _setSettings = null;
    private bool _wifiActive;

    public Camera(BluetoothDevice dev)
    {
        _device = dev;
    }

    public async Task SetupAsync()
    {
        var svcs = await _device.Gatt.GetPrimaryServicesAsync();
        foreach (var svc in svcs)
        {
            var chars = await svc.GetCharacteristicsAsync();
            SetupCharacteristics(chars);
        }

        await ReadDeviceSettings();
    }

    public Task TogglefWifiAP(bool turnOn) => SendCommandAsync(BLE_2_0_Command.APControl, (byte)(turnOn ? 1 : 0));

    public Task ToggleShutter(bool turnOn) => SendCommandAsync(BLE_2_0_Command.SetShutter, (byte)(turnOn ? 1 : 0));

    private static int ReadBytesIntoBuffer(byte[] bytes, List<byte> mBuf)
    {
        int returnLength = -1;
        int startbyte = 1;
        int theseBytes = bytes.Length;
        if ((bytes[0] & 32) > 0)
        {
            //extended 13 bit header
            startbyte = 2;
            int len = (bytes[0] & 0xF) << 8 | bytes[1];
            returnLength = len;
        }
        else if ((bytes[0] & 64) > 0)
        {
            //extended 16 bit header
            startbyte = 3;
            int len = bytes[1] << 8 | bytes[2];
            returnLength = len;
        }
        else if ((bytes[0] & 128) > 0)
        {
            //its a continuation packet
        }
        else
        {
            //8 bit header
            returnLength = bytes[0];
        }
        for (int k = startbyte; k < theseBytes; k++)
            mBuf.Add(bytes[k]);

        return returnLength;
    }

    private static byte[] ReadResponse(byte[] buffer, out BLE_2_0_Command command, out byte responseCode)
    {
        var totalLength = (short)buffer[0];
        var headerLength = 1;
        if (buffer.Length > 256)
        {
            totalLength = BitConverter.ToInt16(buffer, 0);
            headerLength = 2;
        }

        var cmdId = buffer[headerLength];
        command = (BLE_2_0_Command)cmdId;
        responseCode = buffer[headerLength + 1];
        if (totalLength - 2 == 0)
            return new byte[0];

        var response = new byte[totalLength - 2];
        buffer.CopyTo(response, headerLength + 2);
        return response;
    }

    private void NotifyCmds_ValueChanged(object? sender, GattCharacteristicValueChangedEventArgs args)
    {
        ReadResponse(args.Value, out var command, out var responseCode);
        if (responseCode != 0)
        {
            // todo: show error
            return;
        }

        switch (command)
        {
            case BLE_2_0_Command.SetShutter:
                break;

            case BLE_2_0_Command.Sleep:
                break;

            case BLE_2_0_Command.SetDateTime:
                break;

            case BLE_2_0_Command.GetDateTime:
                break;

            case BLE_2_0_Command.SetLivestreamMode:
                break;

            case BLE_2_0_Command.APControl:
                _wifiActive = !_wifiActive;
                APStateChanged?.Invoke(this, _wifiActive);
                break;

            case BLE_2_0_Command.Media_HiLightMoment:
                break;

            case BLE_2_0_Command.Get_Hardware_Info:
                break;

            case BLE_2_0_Command.Presets_LoadGroup:
                break;

            case BLE_2_0_Command.Presets_Load:
                break;

            case BLE_2_0_Command.Analytics:
                break;

            case BLE_2_0_Command.OpenGoPro:
                break;

            default:
                break;
        }
    }

    private async Task ReadDeviceSettings()
    {
        if (_readAPName != null)
        {
            var res = await _readAPName.ReadValueAsync();
            WifiSSID = Encoding.Default.GetString(res);
        }

        if (_readAPPass != null)
        {
            var res = await _readAPPass.ReadValueAsync();
            WifiPassword = Encoding.Default.GetString(res);
        }
    }

    private async Task SendCommandAsync(BLE_2_0_Command command, params byte[] parameter)
    {
        var max = byte.MaxValue;
        if (parameter.Length > max)
            throw new ArgumentOutOfRangeException(nameof(parameter), $"parameter length cannot exceed {max}.");

        List<byte> buf = new();

        // 1 byte for command. 1 byte for parameter length, if length > 0.
        ushort totalLength = (ushort)(1 + (parameter.Length > 0 ? 1 + parameter.Length : 0));
        if (totalLength <= 255)
        {
            buf.Add((byte)totalLength);
        }
        else
        {
            buf.AddRange(BitConverter.GetBytes(totalLength));
        }
        buf.Add((byte)command);
        if (parameter.Length > 0)
        {
            buf.Add((byte)parameter.Length);
            buf.AddRange(parameter);
        }

        if (_sendCmds != null)
        {
            await _sendCmds.WriteValueWithResponseAsync(buf.ToArray());
        }
    }

    private void SetupCharacteristics(IReadOnlyList<GattCharacteristic> list)
    {
        foreach (GattCharacteristic characteristic in list)
        {
            GattCharacteristicProperties properties = characteristic.Properties;
            /*
            if (properties.HasFlag(GattCharacteristicProperties.Read))
            {
                // This characteristic supports reading from it.
            }
            if (properties.HasFlag(GattCharacteristicProperties.Write))
            {
                // This characteristic supports writing to it.
            }
            if (properties.HasFlag(GattCharacteristicProperties.Notify))
            {
                // This characteristic supports subscribing to notifications.
            }
            */
            if (characteristic.Uuid == ReadAPNameGUID)
            {
                _readAPName = characteristic;
            }
            else if (characteristic.Uuid == ReadAPPassGUID)
            {
                _readAPPass = characteristic;
            }
            else if (characteristic.Uuid == SendCommandGUID)
            {
                _sendCmds = characteristic;
            }
            else if (characteristic.Uuid == NotifyCommandGUID)
            {
                _notifyCmds = characteristic;
                _notifyCmds.CharacteristicValueChanged += NotifyCmds_ValueChanged;
                _notifyCmds.StartNotificationsAsync();
            }
            else if (characteristic.Uuid == SetSettingsGUID)
            {
                _setSettings = characteristic;
            }
            else if (characteristic.Uuid == NotifySettingsGUID)
            {
                _notifySettings = characteristic;

                //_notifySettings.CharacteristicValueChanged += //
                //_notifySettings.StartNotificationsAsync();
            }
            else if (characteristic.Uuid == SendQueriesGUID)
            {
                _sendQueries = characteristic;
            }
            else if (characteristic.Uuid == NotifyQueryRespGUID)
            {
                _notifyQueryResp = characteristic;
                /*
                GattCommunicationStatus status = await _notifyQueryResp.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status == GattCommunicationStatus.Success)
                {
                    //mNotifyQueryResp.ValueChanged += MNotifyQueryResp_ValueChanged;
                    if (_sendQueries != null)
                    {
                        //Register for settings and status updates
                        DataWriter mm = new DataWriter();
                        mm.WriteBytes(new byte[] { 1, 0x52 });
                        GattCommunicationStatus gat = await _sendQueries.WriteValueAsync(mm.DetachBuffer());
                        mm = new DataWriter();
                        mm.WriteBytes(new byte[] { 1, 0x53 });
                        gat = await _sendQueries.WriteValueAsync(mm.DetachBuffer());
                    }
                    else
                    {
                        //StatusOutput("send queries was null!");
                    }
                }
                else
                {
                    //failure
                    //StatusOutput("Failed to attach notify query " + status);
                }
                */
            }
        }
    }

    public string? WifiPassword { get; private set; }

    public string? WifiSSID { get; private set; }

    public event EventHandler<bool>? APStateChanged;
}
