using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for StoreHoliday
/// List by Store
/// </summary>
public class StoreHoliday
{
    private string storeID;
    private List<Holiday> holidayList;

    public string StoreID
    {
        get
        {
            return storeID;
        }
    }

    public List<Holiday> HolidayList
    {
        get
        {
            return holidayList;
        }
    }

    public StoreHoliday(string id)
    {
        storeID = id;
        holidayList = new List<Holiday>();
    }

    public void AddHoliday(Holiday holiday)
    {
        holidayList.Add(holiday);
    }

    public void GetStoreHolidayByStoreID(List<StoreHoliday> storeHolidayList, string storeID)
    {
        foreach (StoreHoliday _storeHoliday in storeHolidayList)
        {
            if (_storeHoliday.storeID == storeID)
            {
                holidayList = _storeHoliday.holidayList;
                return;
            }
        }
    }

    public bool IsHolidayByDate(DateTime date)
    {
        foreach (Holiday holiday in holidayList)
        {
            if (holiday.Date == date)
            {
                return true;
            }
        }
        return false;
    }
}


/// <summary>
/// Summary description for Holiday
/// Close Date
/// </summary>
public class Holiday
{
    private DateTime date;
    private bool active;
    private int hType;
    public DateTime Date
    {
        get
        {
            return date;
        }
    }

    public bool Active
    {
        get
        {
            return active;
        }
    }

    public int HType
    {
        get
        {
            return hType;
        }
    }

    public Holiday(DateTime _date, bool _active, int _hType)
    {
        date = _date;
        active = _active;
        hType = _hType;
    }
}
