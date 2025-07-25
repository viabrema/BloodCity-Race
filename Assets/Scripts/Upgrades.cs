using UnityEngine;
using System.Collections;

public class MotorUpgrade
{
    public string name;
    public string description;
    public string type;


    public void execute()
    {
        if (type == "maxSpeed")
        {
            addMaxSpeed();
        }
        else if (type == "acceleration")
        {
            addAcceleration();
        }
        else if (type == "verticalSpeed")
        {
            addVerticalSpeed();
        }

    }
    public void addMaxSpeed()
    {
        RaceManager.Instance.maxSpeed *= 1.03f;
    }

    public void addAcceleration()
    {
        RaceManager.Instance.acceleration += 3f;
    }

    public void addVerticalSpeed()
    {
        RaceManager.Instance.verticalSpeed += 3f;
    }
}

public class nitroUpgrade
{
    public string name;
    public string description;
    public string type;

    public void execute()
    {
        if (type == "nitroDuration")
        {
            addNitroDuration();
        }
        else if (type == "nitroBoost")
        {
            addNitroBoost();
        }
        else if (type == "nitroFrequency")
        {
            nitroFrequency();
        }
    }

    public void addNitroDuration()
    {
        RaceManager.Instance.nitroDuration += 1f;
    }

    public void addNitroBoost()
    {
        RaceManager.Instance.nitroBoost *= 1.1f;
    }

    public void nitroFrequency()
    {
        RaceManager.Instance.nitroFrequency += 0.01f;
    }
}

public class pulseUpgrade
{
    public string name;
    public string description;
    public string type;

    public void execute()
    {
        if (type == "pulseDurability")
        {
            addDurability();
        }
        else if (type == "pulseFrequency")
        {
            pulseFrequency();
        }
    }

    public void addDurability()
    {
        RaceManager.Instance.maxPulseTime += 1;
    }
    public void pulseFrequency()
    {
        RaceManager.Instance.pulseFrequency += 0.01f;
    }
}