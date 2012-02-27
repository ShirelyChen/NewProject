using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for StorePeriods
/// List Periods by Store
/// </summary>
public class StorePeriods
{
    private string storeID;
    private List<Period> arrayOfPeriods;

    public string StoreID
    {
        get
        {
            return storeID;
        }
    }
    public List<Period> ArrayOfPeriods
    {
        get
        {
            return arrayOfPeriods;
        }
    }

    public StorePeriods(string id)
    {
        this.arrayOfPeriods = new List<Period>();
        this.storeID = id;
    }

    public void AddPeriod(Period period)
    {
        arrayOfPeriods.Add(period);
    }

    public void GetStorePeriodByStoreID(List<StorePeriods> storePeriodsList, string storeID)
    {
        foreach (StorePeriods _storePeriods in storePeriodsList)
        {
            if (_storePeriods.storeID == storeID)
            {
                arrayOfPeriods = _storePeriods.ArrayOfPeriods;
                return;
            }
        }
    }
}
/// <summary>
/// Summary description for Period
/// Store Period: Open and Close Hour
/// List by Weekday, Type0--Default,Type 1--Special
/// </summary>
public class Period
{
    private bool defaultHour;
    private DateTime startDate;
    private DateTime endDate;
    private List<StoreDay> storeDayList;

    public List<StoreDay> StoreDayList
    {
        get
        {
            return storeDayList;
        }
    }

    public DateTime StartDate
    {
        get
        {
            return startDate;
        }
    }

    public DateTime EndDate
    {
        get
        {
            return endDate;
        }
    }

    public bool DefaultHour
    {
        get
        {
            return defaultHour;
        }
    }

    public Period(DateTime _startDate, DateTime _endDate, bool _defaultHour)
    {
        storeDayList = new List<StoreDay>();
        startDate = _startDate;
        endDate = _endDate;
        defaultHour = _defaultHour;
    }

    public void AddStoreDay(StoreDay storeDay)
    {
        storeDayList.Add(storeDay);
    }
  
    public StoreDay getHoursByDayOfWeek(DayOfWeek dayOfWeek)
    {
        foreach (StoreDay storeDay in storeDayList)
        {
            if (storeDay.WeekDay == dayOfWeek.GetHashCode())
                return storeDay;
        }
        return null;
    }
}

public class StoreDay
{
    private int weekDay;
    private double openHour;
    private double closeHour;

    public int WeekDay
    {
        get
        {
            return weekDay;
        }
    }

    public double OpenHour
    {
        get
        {
            return openHour;
        }
    }

    public double CloseHour
    {
        get
        {
            return closeHour;
        }
    }

    public StoreDay(int _weekDay, double _openHour, double _closeHour)
    {
        weekDay = _weekDay;
        openHour = _openHour;
        closeHour = _closeHour;
    }
}

public class OpenAndCloseHour
{
    private DateTime day;
    private double openHour;
    private double closeHour;
    public DateTime Day { get { return day; } }
    public double OpenHour { get { return openHour; } }
    public double CloseHour { get { return closeHour; } }

    public OpenAndCloseHour(DateTime _day,StorePeriods _storePeriods,StoreHoliday _storeHoliday)
    {
        day = _day;
        openHour = 0.0;
        closeHour = 0.0;
        foreach (Holiday holiday in _storeHoliday.HolidayList)
        {
            if (holiday.Date == day)
                return;
        }
        if (_storePeriods.ArrayOfPeriods.Count > 1)
        {
            foreach (Period period in _storePeriods.ArrayOfPeriods)
            {
                if (!period.DefaultHour)
                {
                    if ((day > period.StartDate) && (day < period.EndDate))
                    {
                        StoreDay storeDay = period.getHoursByDayOfWeek(day.DayOfWeek);
                        openHour = storeDay.OpenHour;
                        closeHour = storeDay.CloseHour;
                        return;
                    }
                }
            }
        }
        foreach (Period period in _storePeriods.ArrayOfPeriods)
        {
            if (period.DefaultHour == true)
            {
                StoreDay storeDay = period.getHoursByDayOfWeek(day.DayOfWeek);
                openHour = storeDay.OpenHour;
                closeHour = storeDay.CloseHour;
            }
        }
    }
}