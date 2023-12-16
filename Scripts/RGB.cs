using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct RGB
{
    public byte red;
    public byte green;
    public byte blue;
    public byte white;
    /**
        * Initializes red green blue values from a string containing the values.
        * Each three characters are a value.
        * @param aRGBString containing the RGB values.
        */
    public RGB(string aRGBString)
    {
        if (aRGBString.Length >= 9)
        {
            red = byte.Parse(aRGBString.Substring(0, 3));
            green = byte.Parse(aRGBString.Substring(3, 3));
            blue = byte.Parse(aRGBString.Substring(6, 3));
            if (aRGBString.Length >= 12)
            {
                white = byte.Parse(aRGBString.Substring(9, 3));
            }
            else
            {
                white = 0xFF;
            }
        }
        else
        {
            red = 0;
            green = 0;
            blue = 0;
            white = 0;
        }
    }
    /**
        * Initilizes red green and blue with byte values.
        * @param aRed byte value.
        * @param aBlue byte value.
        * @param aGreen byte value.
        */
    public RGB(byte aRed, byte aBlue, byte aGreen)
    {
        red = aRed;
        green = aGreen;
        blue = aBlue;
        white = 0xFF;
    }
    /**
        * Initilizes red green and blue with byte values.
        * @param aRed byte value.
        * @param aBlue byte value.
        * @param aGreen byte value.
        */
    public RGB(byte aRed, byte aBlue, byte aGreen, byte aWhite)
    {
        red = aRed;
        green = aGreen;
        blue = aBlue;
        white = aWhite;
    }
    /**
        * Initilizes red green and blue with hex value.
        * @param aHex
        */
    public RGB(UInt32 aHex)
    {
        red = (byte)(aHex & 0x000000FF);
        green = (byte)((aHex >> 8) & 0x000000FF);
        blue = (byte)((aHex >> 16) & 0x000000FF);
        white = (byte)((aHex >> 24) & 0x000000FF);
    }

    public uint GetHex()
    {
        uint returnVal = (uint)((uint)red | ((uint)green << 8) | ((uint)blue << 16) | ((uint)white << 24));
        return returnVal;
    }

    /**
        * Overrides default ToString function for RGB.
        * @return string with the red green and blue ToString values formatted such that each value has 3 decimal places.
        */
    public override string ToString()
    {
        string iString = red.ToString("D3") + green.ToString("D3") + blue.ToString("D3");
        return iString;
    }
}
