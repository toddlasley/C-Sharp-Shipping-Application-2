﻿// Program 1A
// CIS 200-10
// Summer 2015
// Due: 5/31/2015
// By: Andrew L. Wright

// File: AirPackage.cs
// The AirPackage class is an abstract derived class from Package. It is able
// to determine if the package is heavy or large.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AirPackage : Package
{
    public const double HEAVY_THRESHOLD = 75;  // Min weight of heavy package
    public const double LARGE_THRESHOLD = 100; // Min dimensions of large package

    // Precondition:  pLength > 0, pWidth > 0, pHeight > 0,
    //                pWeight > 0
    // Postcondition: The air package is created with the specified values for
    //                origin address, destination address, length, width,
    //                height, and weight
    public AirPackage(Address originAddress, Address destAddress,
        double pLength, double pWidth, double pHeight, double pWeight)
        : base(originAddress, destAddress, pLength, pWidth, pHeight, pWeight)
    {
        // All work done in base class constructor
    }

    // Precondition:  None
    // Postcondition: Returns true if air package is considered heavy
    //                else returns false
    public bool IsHeavy()
    {
        return (Weight >= HEAVY_THRESHOLD);
    }

    // Precondition:  None
    // Postcondition: Returns true if air package is considered large
    //                else returns false
    public bool IsLarge()
    {
        return (TotalDimension >= LARGE_THRESHOLD);
    }

    // Precondition:  None
    // Postcondition: A String with the air package's data has been returned
    public override string ToString()
    {
        return String.Format("Air{0}{3}IsHeavy: {1}{3}IsLarge: {2}",
            base.ToString(), IsHeavy(), IsLarge(), System.Environment.NewLine);
    }
}
