using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping.UPS
{
    public class Address
    {
        public List<string> AddressLine { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }

    public class BillShipper
    {
        public string AccountNumber { get; set; }
    }

    public class CustomerClassification
    {
        public string Code { get; set; }
    }

    public class DeliveryTimeInformation
    {
        public string PackageBillType { get; set; }
        public Pickup Pickup { get; set; }
    }

    public class Dimensions
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

    public class InvoiceLineTotal
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class Package
    {
        public PackagingType PackagingType { get; set; }
        public Dimensions Dimensions { get; set; }
        public PackageWeight PackageWeight { get; set; }
    }

    public class PackageWeight
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }

    public class PackagingType
    {
        public string Code { get; set; }
    }

    public class PaymentDetails
    {
        public ShipmentCharge ShipmentCharge { get; set; }
    }

    public class Pickup
    {
        public string Date { get; set; }
    }

    public class PickupType
    {
        public string Code { get; set; }
    }

    public class RateRequest
    {
        public PickupType PickupType { get; set; }
        public CustomerClassification CustomerClassification { get; set; }
        public Shipment Shipment { get; set; }
    }

    public class UPSQuoteRequest
    {
        public RateRequest RateRequest { get; set; }
    }

    public class Shipment
    {
        public Shipper Shipper { get; set; }
        public ShipTo ShipTo { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public List<Package> Package { get; set; }
        public ShipmentRatingOptions ShipmentRatingOptions { get; set; }
        public int NumOfPieces { get; set; }
        public ShipmentTotalWeight ShipmentTotalWeight { get; set; }
        public InvoiceLineTotal InvoiceLineTotal { get; set; }
        public DeliveryTimeInformation DeliveryTimeInformation { get; set; }
    }

    public class ShipmentCharge
    {
        public string Type { get; set; }
        public BillShipper BillShipper { get; set; }
    }

    public class ShipmentRatingOptions
    {
        public string NegotiatedRatesIndicator { get; set; }
    }

    public class ShipmentTotalWeight
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }

    public class Shipper
    {
        public string ShipperNumber { get; set; }
        public Address Address { get; set; }
    }

    public class ShipTo
    {
        public Address Address { get; set; }
    }

    public class UnitOfMeasurement
    {
        public string Code { get; set; }
    }
}
