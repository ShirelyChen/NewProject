using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DailyData
/// List by Stores
/// </summary>
/// 
public class Stores
{
    private List<string> storeList;
    public List<string> StoreList
    {
        get
        {
            return storeList;
        }
    }

    public Stores()
    {
        storeList = new List<string>();
    }

    public bool Contains(string storeName)
    {
        return storeList.Contains(storeName);
    }

    public void Add(string storeName)
    {
        storeList.Add(storeName);
    }

    public bool Remove(string storeName)
    {
        return storeList.Remove(storeName);
    }
}

public class StoreTotal<T>
{
    private string storeID;
    public string StoreID
    {
        get { return storeID; }
    }
    private T totalValue;
    public T TotalValue
    {
        get { return totalValue; }
    }
    private int totalHour;
    public int TotalHour
    {
        get { return totalHour; }
    }
    public StoreTotal()
    {
        storeID = "";
        totalValue = default(T);
        totalHour = 0;
    }
    public StoreTotal(string _storeID)
    {
        storeID = _storeID;
        totalValue = default(T);
        totalHour = 0;
    }
    public StoreTotal( T _totalValue, int _totalHour)
    {
        storeID = "";
        totalValue = _totalValue;
        totalHour = _totalHour;
    }
    public StoreTotal(string _storeID,T _totalValue,int _totalHour)
    {
        storeID = _storeID;
        totalValue = _totalValue;
        totalHour = _totalHour;
    }
    public void Add(StoreTotal<T> _storeTotal,Calculator<T> calculator)
    {
        totalValue = calculator.Add(totalValue, _storeTotal.totalValue);
        totalHour += _storeTotal.totalHour;
    }
}

public class StoreDailyDataList<T>
{
    private string storeID;
    private List<DailyData<T>> dailyDataList;
    public string StoreID
    {
        get
        {
            return storeID;
        }
    }
    public List<DailyData<T>> DailyDataList
    {
        get
        {
            return dailyDataList;
        }
    }
    public StoreDailyDataList(string _storeID)
    {
        storeID = _storeID;
        dailyDataList = new List<DailyData<T>>();
    }
    public void AddDailyData(DailyData<T> dailyData)
    {
        dailyDataList.Add(dailyData);
    }
    public void RemoveDailyData(DailyData<T> dailyData)
    {
        dailyDataList.Remove(dailyData);   
    }
    public StoreTotal<T> GetStoreTotalByOpenAndCloseHour(StorePeriods storePeriods, StoreHoliday storeHoliday,Calculator<T> calculator)
    { 
        StoreTotal<T> storeTotal=new StoreTotal<T>(storeID);
        foreach (DailyData<T> dailyData in dailyDataList)
        {
            storeTotal.Add(dailyData.GetStoreDailyTotalByOpenAndCloseHour(storePeriods, storeHoliday, calculator),calculator);
        }
        return storeTotal;
    }

}

public class DailyData<T> 
{
    private DateTime day;
    private List<HourlyData<T>> hourlyList;
    public DateTime Day
    {
        get
        {
            return day;
        }
    }
    public List<HourlyData<T>> HourlyList
    {
        get
        {
            return hourlyList;
        }
    }
    public DailyData(DateTime _day)
    {
        day = _day;
        hourlyList = new List<HourlyData<T>>();
    }
    public void AddHourlyData(HourlyData<T> hourlyData)
    {
        hourlyList.Add(hourlyData);
    }
    public void RemoveHourlyData(HourlyData<T> hourlydata)
    {
        hourlyList.Remove(hourlydata);
    }
    public StoreTotal<T> GetStoreDailyTotalByOpenAndCloseHour(StorePeriods storePeriods, StoreHoliday storeHoliday,Calculator<T> calculator)
    { 
        T totalValue=default(T);
        int totalHour=0;
        StoreTotal<T> storeTotal;
        OpenAndCloseHour openAndCloseHour = new OpenAndCloseHour(day, storePeriods, storeHoliday);
        if ((Convert.ToInt32(openAndCloseHour.OpenHour)==0)&&(Convert.ToInt32(openAndCloseHour.CloseHour)==0))
        {
            storeTotal=new StoreTotal<T>(totalValue,totalHour);
            return storeTotal;
        }
        int Open=Convert.ToInt32(openAndCloseHour.OpenHour);
        int Close = Convert.ToInt32(openAndCloseHour.CloseHour)+1;
        foreach (HourlyData<T> hourlyData in hourlyList)
        {
            if ((hourlyData.Hour >= Open) && (hourlyData.Hour <= Close))
            {
                totalHour++;
                totalValue = calculator.Add(totalValue , hourlyData.Data);
            }
        }
        storeTotal = new StoreTotal<T>(totalValue, totalHour);
        return storeTotal;
    }

}
/// <summary>
/// Summary description for HourlyData
/// Hour: 0-23
/// </summary>
public class HourlyData<T>
{
    private T data;
    private int hour;
    public T Data
    {
        get
        {
            return data;
        }
    }
    public int Hour
    {
        get
        {
            return hour;
        }
    }
    public HourlyData(T _data, int _hour)
    {
        data = _data;
        hour = _hour;
    }

}


public abstract class Calculator<T>
{
    public abstract T Add(T a, T b);
}
public class CalculatorInt : Calculator<int>
{
    public override int Add(int a, int b)
    {
        return a + b;
    }
}
public class CalculatorDouble : Calculator<double>
{
    public override double Add(double a, double b)
    {
        return a + b;
    }
}




