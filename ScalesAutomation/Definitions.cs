using System;
using System.Collections.Generic;

namespace ScalesAutomation
{

    public struct Measurement
    {
        public bool isStable;
        public int weight;
    }

    public struct LotInfo
    {
        public String Lot;
        public String ProductName;
        public Package Package;
    }

    public struct Package
    {
        public String Type;
        public String Tare;
        public String NetWeight;
    }

    public struct Product
    {
        public String Name;
        public List<Package> PackageDetails;
    }

    enum State
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