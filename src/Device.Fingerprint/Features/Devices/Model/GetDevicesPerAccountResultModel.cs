namespace Device.Fingerprint.Features.Devices.Model
{
    public class GetDevicesPerAccountResultModel
    {
        public IEnumerable<GetDevicesPerAccountItemModel> Items { get; set; }

        public int TotalRows { get; set; }
    }
}
