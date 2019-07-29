using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class CommonIdentity
    {
        public string Keyword { get; set; }

        public int TotalCount { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortField { get; set; }

        public string SortType { get; set; }
    }

    #region Meta data

    #region Shop

    public class MetaDataShop
    {
        public int ShopId { get; set; }
        public string Amenities { get; set; }
        public string NearPlaces { get; set; }
        public string Policies { get; set; }
        public string Payments { get; set; }
    }

    public class MetaShopImage
    {
        public string Id { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaShopAmenity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaShopNearPlace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Range { get; set; }
        public string UnitCode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaShopReview
    {
        public string Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public decimal Rating { get; set; }
        public string Message { get; set; }
        public int TravellerTypeId { get; set; }
        public string ReviewImages { get; set; }
        public string ReviewCategories { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaShopPolicy
    {
        public string Id { get; set; }
        public int PolicyId { get; set; }
        public string PolicyCode { get; set; }
        public string PolicyData { get; set; }
    }

    public class MetaPolicyDataCheckInOut
    {
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaChildCommon
    {
        public int AcceptGuest { get; set; }
        public int ChildrenOver { get; set; }
        public string PolicyDes { get; set; }
        public List<MetaPolicyDataChildAndBeds> ChildrenExtraBeds { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaPolicyDataChildAndBeds
    {
        public string Id { get; set; }
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }
        public int ExtraInfantBed { get; set; }
        public decimal ExtraInfantBedSurcharge { get; set; }
        public int BedShare { get; set; }
        public decimal BedShareSurcharge { get; set; }
        public int BreakFast { get; set; }
        public decimal BreakFastSurcharge { get; set; }
        public string CurrencyCode { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaPolicyDataPets
    {
        public int Accepted { get; set; }
        public int Pets { get; set; }
        public string Note { get; set; }
    }

    public class MetaPolicyDataDinning
    {
        public string Id { get; set; }
        public int DinningType { get; set; }
        public int IsBuffet { get; set; }
        public decimal Surcharge { get; set; }
        public string CurrencyCode { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaPolicyDataCreditCard
    {
        public string CreditCards { get; set; }
        public string Note { get; set; }
    }

    public class MetaPolicyDataPayment
    {
        public string Id { get; set; }
        public string PaymentId { get; set; }
        public string PaymentName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    #endregion

    #region Room

    public class MetaDataRoom
    {
        public int RoomId { get; set; }
        public string BedInfo { get; set; }
        public string SizeInfo { get; set; }           
        public string Policies { get; set; }
        public string Payments { get; set; }
        public string BathroomInfo { get; set; }
        public string Amenities { get; set; }
    }

    public class MetaRoomImage
    {
        public string Id { get; set; }
        public int ShopId { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomBedInfo
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int BedNumber { get; set; }
        public int BedType { get; set; }
        public decimal BedWidth { get; set; }
        public decimal BedLength { get; set; }
        public string UnitCode { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomExtraBeds
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int BedNumber { get; set; }
        public decimal BedWidth { get; set; }
        public decimal BedLength { get; set; }
        public string UnitCode { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomSizeInfo
    {
        public decimal SizeFrom { get; set; }
        public decimal SizeTo { get; set; }
        public string Note { get; set; }
        public string UnitCode { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomFloorInfo
    {
        public string Id { get; set; }
        public string FloorNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomPolicy
    {
        public string Id { get; set; }
        public int PolicyId { get; set; }
        public string PolicyType { get; set; }
        public string PolicyName { get; set; }
    }

    public class MetaRoomPayment
    {
        public string Id { get; set; }
        public int PaymentId { get; set; }
        public string PaymentName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomBathroomInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MetaRoomAmenity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    #endregion

    #endregion
}
