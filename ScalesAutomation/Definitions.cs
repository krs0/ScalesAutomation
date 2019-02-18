using System;
using System.Collections.Generic;

namespace ScalesAutomation
{

    public struct Measurement
    {
        public bool IsStable;
        public int TotalWeight;
        public DateTime TimeStamp;
    }

    public struct Package
    {
        public String Type;
        public Double Tare;
        public Double NetWeight;
        public Double TotalWeight; // = NetWeight + Tare
    }


    public struct Product
    {
        public String Name;
        public List<Package> PackageDetails;
    }

    enum MeasurementState
    {
        stable = 48,
        unstable = 49,
        notValid = 51
    };

    enum Units
    {
        kg = 45
    };

}