using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for DBConnection
/// </summary>

public enum DataType { Traffic, Transaction, Sales, StaffHours, Other }

public class DBConnection
{
    ReadInfo ConInfo = new ReadInfo();
    public SqlConnection con;
    public SqlCommand cmd;

    public DBConnection(string sDB)
    {
        ConInfo.ReadConnection(1);
        con = new SqlConnection(setConnection(sDB));
        cmd = new SqlCommand();
        cmd.Connection = con;
    }

    private string setConnection(string sDB)
    {
        string conString = "";
        conString = "Server=" + ConInfo.server;
        conString += ";Database=" + sDB;
        conString += ";UID=" + ConInfo.user;
        conString += ";Pwd=" + ConInfo.pass;
        conString += ";Trusted_Connection = yes;";
        return conString;
    }
}

public class DataFromDatabase<T>
{
    DBConnection dbCon;
    SqlConnection con;
    SqlCommand cmd;
    public DataFromDatabase(string strDB)
    {
        dbCon=new DBConnection(strDB);
        con=dbCon.con;
        cmd = dbCon.cmd;
    }
    public StoreDailyDataList<T> GetHourlyDataByStore(string strStoreID, DateTime startDate, DateTime endDate, DataType dataType)
    {
        StoreDailyDataList<T> storeDailyDataList = new StoreDailyDataList<T>(strStoreID);
        string sSql = "";
        if (dataType == DataType.Traffic)
        {
            sSql = "SELECT  Devices.StoreNo, trfXXXX.lDate, SUM(trfXXXX.DayTotal), SUM(trfXXXX.c1200AM), SUM(trfXXXX.c0100AM), SUM(trfXXXX.c0200AM), SUM(trfXXXX.c0300AM), ";
            sSql += " SUM(trfXXXX.c0400AM), SUM(trfXXXX.c0500AM), SUM(trfXXXX.c0600AM), SUM(trfXXXX.c0700AM), SUM(trfXXXX.c0800AM), SUM(trfXXXX.c0900AM), ";
            sSql += " SUM(trfXXXX.c1000AM), SUM(trfXXXX.c1100AM), SUM(trfXXXX.c1200PM), SUM(trfXXXX.c0100PM), SUM(trfXXXX.c0200PM), SUM(trfXXXX.c0300PM),";
            sSql += " SUM(trfXXXX.c0400PM), SUM(trfXXXX.c0500PM), SUM(trfXXXX.c0600PM), SUM(trfXXXX.c0700PM), SUM(trfXXXX.c0800PM), SUM(trfXXXX.c0900PM), ";
            sSql += " SUM(trfXXXX.c1000PM), SUM(trfXXXX.c1100PM) FROM Devices INNER JOIN trfXXXX ON trfXXXX.DeviceNo = Devices.DeviceNo Inner join Entrances On Devices.DeviceNo=Entrances.DeviceNo";
            sSql += " WHERE (Devices.StoreNo = '" + strStoreID + "') and TrfXXXX.lDate>=" + startDate.Ticks.ToString() + " and trfXXXX.lDate<=" + endDate.Ticks.ToString();
            sSql += " and Entrances.EntranceNo=0 GROUP BY Devices.StoreNo, trfXXXX.lDate ";
        }
        else
        {
            string strTableName;
            if (dataType == DataType.Transaction)
                strTableName = "trnXXXX";
            else if (dataType == DataType.Sales)
                strTableName = "slsXXXX";
            else if (dataType == DataType.StaffHours)
                strTableName = "hrsXXXX";
            else
                strTableName = "othXXXX";

            sSql = "SELECT  StoreNo, lDate, DayTotal, c1200AM, c0100AM, c0200AM, c0300AM, c0400AM, c0500AM, c0600AM,";
            sSql += " c0700AM, c0800AM, c0900AM, c1000AM, c1100AM, c1200PM, c0100PM, c0200PM, c0300PM, c0400PM,";
            sSql += " c0500PM, c0600PM, c0700PM, c0800PM, c0900PM, c1000PM, c1100PM FROM " + strTableName;
            sSql += " WHERE StoreNo = '" + strStoreID + "' and lDate>=" + startDate.Ticks.ToString() + " and lDate<=" + endDate.Ticks.ToString();
        }
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                long lDate = Convert.ToInt64(sdr["lDate"].ToString());
                DailyData<T> dailyData = new DailyData<T>(new DateTime(lDate));
                for (int iHour = 0; iHour < 24; iHour++)
                {
                    string s = sdr[3 + iHour].ToString();
                    T data = (T)Convert.ChangeType(s, typeof(T));
                    if (Convert.ToInt32(data) != 0)
                    {
                        HourlyData<T> hourlyData = new HourlyData<T>(data, iHour);
                        dailyData.AddHourlyData(hourlyData);
                    }
                }
                storeDailyDataList.AddDailyData(dailyData);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch { }
        finally
        {
            con.Close();
        }
        return storeDailyDataList;
    }
    public List<StoreDailyDataList<T>> GetAllStoreHourlyData(DateTime startDate, DateTime endDate, DataType dataType)
    {
        List<StoreDailyDataList<T>> storesListDailyDataList = new List<StoreDailyDataList<T>>();
        string sSql = "";
        if (dataType == DataType.Traffic)
        {
            sSql = "SELECT  Devices.StoreNo, trfXXXX.lDate, SUM(trfXXXX.DayTotal), SUM(trfXXXX.c1200AM), SUM(trfXXXX.c0100AM), SUM(trfXXXX.c0200AM), SUM(trfXXXX.c0300AM), ";
            sSql += " SUM(trfXXXX.c0400AM), SUM(trfXXXX.c0500AM), SUM(trfXXXX.c0600AM), SUM(trfXXXX.c0700AM), SUM(trfXXXX.c0800AM), SUM(trfXXXX.c0900AM), ";
            sSql += " SUM(trfXXXX.c1000AM), SUM(trfXXXX.c1100AM), SUM(trfXXXX.c1200PM), SUM(trfXXXX.c0100PM), SUM(trfXXXX.c0200PM), SUM(trfXXXX.c0300PM),";
            sSql += " SUM(trfXXXX.c0400PM), SUM(trfXXXX.c0500PM), SUM(trfXXXX.c0600PM), SUM(trfXXXX.c0700PM), SUM(trfXXXX.c0800PM), SUM(trfXXXX.c0900PM), ";
            sSql += " SUM(trfXXXX.c1000PM), SUM(trfXXXX.c1100PM) FROM Devices INNER JOIN trfXXXX ON trfXXXX.DeviceNo = Devices.DeviceNo Inner join Entrances On Devices.DeviceNo=Entrances.DeviceNo ";
            sSql += " WHERE TrfXXXX.lDate>=" + startDate.Ticks.ToString() + " and trfXXXX.lDate<=" + endDate.Ticks.ToString();
            sSql += " and Entrances.EntranceNo=0 GROUP BY Devices.StoreNo, trfXXXX.lDate ";
            sSql += " Order by Devices.StoreNo";
        }
        else
        {
            string strTableName;
            if (dataType == DataType.Transaction)
                strTableName = "trnXXXX";
            else if (dataType == DataType.Sales)
                strTableName = "slsXXXX";
            else if (dataType == DataType.StaffHours)
                strTableName = "hrsXXXX";
            else
                strTableName = "othXXXX";

            sSql = "SELECT  StoreNo, lDate, DayTotal, c1200AM, c0100AM, c0200AM, c0300AM, c0400AM, c0500AM, c0600AM,";
            sSql += " c0700AM, c0800AM, c0900AM, c1000AM, c1100AM, c1200PM, c0100PM, c0200PM, c0300PM, c0400PM,";
            sSql += " c0500PM, c0600PM, c0700PM, c0800PM, c0900PM, c1000PM, c1100PM FROM " + strTableName;
            sSql += " WHERE lDate>=" + startDate.Ticks.ToString() + " and lDate<=" + endDate.Ticks.ToString();
            sSql += " Order by StoreNo";
        }
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            string newStoreName = "";
            StoreDailyDataList<T> storeDailyDataList = null;
            while (sdr.Read())
            {
                if ((newStoreName != sdr[0].ToString()) || (storeDailyDataList == null))
                {
                    if (storeDailyDataList != null)
                    {
                        storesListDailyDataList.Add(storeDailyDataList);
                    }
                    storeDailyDataList = new StoreDailyDataList<T>(sdr[0].ToString());
                }
                
                long lDate = Convert.ToInt64(sdr["lDate"].ToString());
                DailyData<T> dailyData = new DailyData<T>(new DateTime(lDate));
                for (int iHour = 0; iHour < 24; iHour++)
                {
                    string s = sdr[3 + iHour].ToString();
                    T data = (T)Convert.ChangeType(s,typeof(T));
                    if (Convert.ToInt32(data) != 0)
                    {
                        HourlyData<T> hourlyData = new HourlyData<T>(data, iHour);
                        dailyData.AddHourlyData(hourlyData);
                    }
                }
                newStoreName = sdr[0].ToString();
                storeDailyDataList.AddDailyData(dailyData);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch { }
        finally
        {
            con.Close();
        }
        return storesListDailyDataList;
    }
    public List<StoreDailyDataList<T>> GetHourlyDataByLocation(int locationType,string[] locationListID, DateTime startDate, DateTime endDate, DataType dataType)
    {
        List<StoreDailyDataList<T>> storeListDailyDataList = new List<StoreDailyDataList<T>>();
        string sSql = "";
        string strLocationType = "";
        string CondLocationType = "";
        string CondLocationID = "";
        switch (locationType)
        {
            case 1:
                {
                    strLocationType = "Divisions";
                    CondLocationType += " Inner join Districts On Districts.District=Stores.District ";
                    CondLocationType += " Inner Join Regions On Regions.Region=Districts.Region ";
                    CondLocationType += " Inner Join Divisions On Divisions.Division=Regions.Division ";
                    CondLocationID="Divisions.Division = '"+locationListID[0]+"' ";
                    for(int i=1;i<locationListID.Length;i++)
                    {
                         CondLocationID+=" Or Divisions.Division = '"+locationListID[i]+"' ";
                    }
                    break;
                }
            case 2:
                {
                    strLocationType = "Regions";
                    CondLocationType += " Inner join Districts On Districts.District=Stores.District ";
                    CondLocationType += " Inner Join Regions On Regions.Region=Districts.Region ";
                    CondLocationID = "Regions.Region = '" + locationListID[0] + "' ";
                    for (int i = 1; i < locationListID.Length; i++)
                    {
                        CondLocationID += " Or Regions.Region = '" + locationListID[i] + "' ";
                    }
                    break;
                }
            case 3:
                {
                    strLocationType = "Districts";
                    CondLocationType += " Inner join Districts On Districts.District=Stores.District ";
                    CondLocationID = "Districts.District = '" + locationListID[0] + "' ";
                    for (int i = 1; i < locationListID.Length; i++)
                    {
                        CondLocationID += " Or Districts.District = '" + locationListID[i] + "' ";
                    }
                    break;
                }
            case 4:
                {
                    strLocationType = "Stores";
                    CondLocationID = "Stores.StoreNo = '" + locationListID[0] + "' ";
                    for (int i = 1; i < locationListID.Length; i++)
                    {
                        CondLocationID += " Or Stores.StoreNo = '" + locationListID[i] + "' ";
                    }
                    break;
                }
        }



        if (dataType == DataType.Traffic)
        {
            sSql = "SELECT  Devices.StoreNo, trfXXXX.lDate, SUM(trfXXXX.DayTotal), SUM(trfXXXX.c1200AM), SUM(trfXXXX.c0100AM), SUM(trfXXXX.c0200AM), SUM(trfXXXX.c0300AM), ";
            sSql += " SUM(trfXXXX.c0400AM), SUM(trfXXXX.c0500AM), SUM(trfXXXX.c0600AM), SUM(trfXXXX.c0700AM), SUM(trfXXXX.c0800AM), SUM(trfXXXX.c0900AM), ";
            sSql += " SUM(trfXXXX.c1000AM), SUM(trfXXXX.c1100AM), SUM(trfXXXX.c1200PM), SUM(trfXXXX.c0100PM), SUM(trfXXXX.c0200PM), SUM(trfXXXX.c0300PM),";
            sSql += " SUM(trfXXXX.c0400PM), SUM(trfXXXX.c0500PM), SUM(trfXXXX.c0600PM), SUM(trfXXXX.c0700PM), SUM(trfXXXX.c0800PM), SUM(trfXXXX.c0900PM), ";
            sSql += " SUM(trfXXXX.c1000PM), SUM(trfXXXX.c1100PM) FROM trfXXXX INNER JOIN Devices ON trfXXXX.DeviceNo = Devices.DeviceNo Inner join Entrances On Devices.DeviceNo=Entrances.DeviceNo Inner join Stores On Stores.StoreNo=Devices.StoreNo " + CondLocationType;
            sSql += " WHERE ("+CondLocationID+") and (TrfXXXX.lDate>=" + startDate.Ticks.ToString() + " and trfXXXX.lDate<=" + endDate.Ticks.ToString()+") and Entrances.EntranceNo=0 ";
            sSql += " GROUP BY Devices.StoreNo, trfXXXX.lDate Order by Devices.StoreNo";
        }
        else
        {
            string strTableName;
            if (dataType == DataType.Transaction)
                strTableName = "trnXXXX";
            else if (dataType == DataType.Sales)
                strTableName = "slsXXXX";
            else if (dataType == DataType.StaffHours)
                strTableName = "hrsXXXX";
            else
                strTableName = "othXXXX";

            sSql = "SELECT  " + strTableName + ".StoreNo, lDate, DayTotal, c1200AM, c0100AM, c0200AM, c0300AM, c0400AM, c0500AM, c0600AM, c0700AM, c0800AM, c0900AM, c1000AM,";
            sSql += " c1100AM, c1200PM, c0100PM, c0200PM, c0300PM, c0400PM, c0500PM, c0600PM, c0700PM, c0800PM, c0900PM, c1000PM, c1100PM";
            sSql += " FROM " + strTableName + " Inner join Stores On Stores.StoreNo="+strTableName+".StoreNo " + CondLocationType;
            sSql += " WHERE (" + CondLocationID + ") and lDate>=" + startDate.Ticks.ToString() + " and lDate<=" + endDate.Ticks.ToString();
            sSql += " Order by "+strTableName+".StoreNo" ;
        }
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            string newStoreName = "";
            StoreDailyDataList<T> storeDailyDataList = null;
            while (sdr.Read())
            {
                if ((newStoreName != sdr[0].ToString()) || (storeDailyDataList == null))
                {
                    if (storeDailyDataList != null)
                    {
                        storeListDailyDataList.Add(storeDailyDataList);
                    }
                    storeDailyDataList = new StoreDailyDataList<T>(sdr[0].ToString());
                }

                long lDate = Convert.ToInt64(sdr["lDate"].ToString());
                DailyData<T> dailyData = new DailyData<T>(new DateTime(lDate));
                for (int iHour = 0; iHour < 24; iHour++)
                {
                    string s = sdr[3 + iHour].ToString();
                    T data = (T)Convert.ChangeType(s, typeof(T));
                    if (Convert.ToInt32(data) != 0)
                    {
                        HourlyData<T> hourlyData = new HourlyData<T>(data, iHour);
                        dailyData.AddHourlyData(hourlyData);
                    }
                }
                newStoreName = sdr[0].ToString();
                storeDailyDataList.AddDailyData(dailyData);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch { }
        finally
        {
            con.Close();
        }
        return storeListDailyDataList;
    }
}
public class PeriodsFromDB
{
     DBConnection dbCon;
    SqlConnection con;
    SqlCommand cmd;
    public PeriodsFromDB(string strDB)
    {
        dbCon=new DBConnection(strDB);
        con=dbCon.con;
        cmd = dbCon.cmd;
    }
    public StorePeriods GetHourListByStore(string strStoreID)
    {
        StorePeriods storePeriods = new StorePeriods(strStoreID);
        bool bDefault;
        DateTime PeriodBegin, PeriodEnd;
        double Open, Close;
        Period period;
        StoreDay storeDay;
        string sSql = "Select  StoreNo, DefaultHour, PeriodBegin, PeriodEnd, SunOpen, SunClose, MonOpen, MonClose, ";
        sSql += " TueOpen, TueClose, WedOpen, WedClose, ThuOpen,  ThuClose, FriOpen, FriClose, SatOpen, SatClose ";
        sSql += " from StoreHour where StoreNo='" + strStoreID + "' order by DefaultHour";
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                if (sdr["DefaultHour"].ToString() == "0")
                    bDefault = true;
                else
                    bDefault = false;
                PeriodBegin = new DateTime(Convert.ToInt64(sdr["Periodbegin"]));
                PeriodEnd = new DateTime(Convert.ToInt64(sdr["PeriodEnd"]));
                period = new Period(PeriodBegin, PeriodEnd, bDefault);
                for (int iDay = 0; iDay < 7; iDay++)
                {
                    Open = Double.Parse(sdr[2 * iDay + 4].ToString());
                    Close = Double.Parse(sdr[2 * iDay + 5].ToString());
                    storeDay = new StoreDay(iDay, Open, Close);
                    period.AddStoreDay(storeDay);
                }
                storePeriods.AddPeriod(period);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch
        { }
        finally
        {
            con.Close();
        }
        return storePeriods;
    }

    public List<StorePeriods> GetAllStoresHourList()
    {
        List<StorePeriods> storePeriodsList = new List<StorePeriods>();
        bool bDefault;
        DateTime PeriodBegin, PeriodEnd;
        double Open, Close;
        Period period;
        StoreDay storeDay;
        string sSql = "Select  StoreNo, DefaultHour, PeriodBegin, PeriodEnd, SunOpen, SunClose, MonOpen, MonClose, ";
        sSql += " TueOpen, TueClose, WedOpen, WedClose, ThuOpen,  ThuClose, FriOpen, FriClose, SatOpen, SatClose ";
        sSql += " from StoreHour order by StoreNo, DefaultHour";

        try
        {

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            string strNewStoreID = "";
            StorePeriods storePeriods = null;
            while (sdr.Read())
            {
                if ((strNewStoreID!=sdr[0].ToString())||(storePeriods==null))
                {
                    if (storePeriods != null)
                        storePeriodsList.Add(storePeriods);
                    storePeriods=new StorePeriods(sdr[0].ToString());
                }
                if (sdr["DefaultHour"].ToString() == "0")
                    bDefault = true;
                else
                    bDefault = false;
                PeriodBegin = new DateTime(Convert.ToInt64(sdr["Periodbegin"]));
                PeriodEnd = new DateTime(Convert.ToInt64(sdr["PeriodEnd"]));
                period = new Period(PeriodBegin, PeriodEnd, bDefault);
                for (int iDay = 0; iDay < 7; iDay++)
                {
                    Open = Double.Parse(sdr[2 * iDay + 4].ToString());
                    Close = Double.Parse(sdr[2 * iDay + 5].ToString());
                    storeDay = new StoreDay(iDay, Open, Close);
                    period.AddStoreDay(storeDay);
                }
                strNewStoreID = sdr[0].ToString();
                storePeriods.AddPeriod(period);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch
        { }
        finally
        {
            con.Close();
        }
        return storePeriodsList;
    }
}

public class HolidaysFromDB
{
     DBConnection dbCon;
    SqlConnection con;
    SqlCommand cmd;
    public HolidaysFromDB(string strDB)
    {
        dbCon=new DBConnection(strDB);
        con=dbCon.con;
        cmd = dbCon.cmd;
    }
    public StoreHoliday GetHolidayListByStore(string strStoreID)
    {
        StoreHoliday storeHoliday = new StoreHoliday(strStoreID);
        DateTime date;
        bool active;
        int hType;
        Holiday holiday;
        string sSql = "SELECT EId, StoreNo, EventDay, Active, Description, hType  ";
        sSql += " FROM StoreEvents where StoreNo='" + strStoreID + "'";
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                date = new DateTime(Convert.ToInt64(sdr["EventDay"]));
                active = Convert.ToBoolean(sdr["Active"]);
                hType = Convert.ToInt32(sdr["hType"]);
                holiday = new Holiday(date, active, hType);
                storeHoliday.AddHoliday(holiday);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch
        { }
        finally
        {
            con.Close();
        }
        return storeHoliday;
    }

    public List<StoreHoliday> GetAllStoresHolidayList()
    {
        List<StoreHoliday> storeHolidayList = new List<StoreHoliday>();
        DateTime date;
        bool active;
        int hType;
        Holiday holiday;
        string sSql = "SELECT EId, StoreNo, EventDay, Active, Description, hType  ";
        sSql += " FROM StoreEvents order by StoreNo";
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandText = sSql;
            SqlDataReader sdr = cmd.ExecuteReader();
            string strNewStore = "";
            StoreHoliday storeHoliday = null;
            while (sdr.Read())
            {
                if ((sdr["StoreNo"].ToString() != strNewStore) || (storeHoliday == null))
                {
                    if (storeHoliday != null)
                        storeHolidayList.Add(storeHoliday);
                    storeHoliday=new StoreHoliday(sdr["StoreNo"].ToString());
                }
                date = new DateTime(Convert.ToInt64(sdr["EventDay"]));
                active = Convert.ToBoolean(sdr["Active"]);
                hType = Convert.ToInt32(sdr["hType"]);
                holiday = new Holiday(date, active, hType);
                strNewStore = sdr["StoreNo"].ToString();
                storeHoliday.AddHoliday(holiday);
            }
            sdr.Close();
            sdr.Dispose();
        }
        catch
        { }
        finally
        {
            con.Close();
        }
        return storeHolidayList;
    }
}

public class StoresFromDB
{
      DBConnection dbCon;
    SqlConnection con;
    SqlCommand cmd;
    public StoresFromDB(string strDB)
    {
        dbCon=new DBConnection(strDB);
        con=dbCon.con;
        cmd = dbCon.cmd;
    }
    public Stores GetStoreList(string[] sList, int iLoc)
    {
        Stores stores = new Stores();
        if (iLoc == 4)
        {
            for (int i = 0; i < sList.Length; i++)
            {
                stores.Add(sList[i]);
            }
            return stores;
        }
        int iCond = -1;
        string sSql = "", sCond = "";
        for (int i = 0; i < sList.Length; i++)
        {
            if (iLoc == 1)
            {
                sCond += " Div.Division=" + sList[i] + " or ";
                iCond = 36;
            }
            else if (iLoc == 2)
            {
                sCond += " reg.Region=" + sList[i] + " or ";
                iCond = 34;
            }
            else if (iLoc == 3)
            {
                sCond += " Dis.District=" + sList[i] + " or ";
                iCond = 32;
            }
            else if (iLoc == 4)
            {
                sCond += " s.StoreNo ='" + sList[i] + "' or ";
                iCond = 5;
            }
        }
        if (iCond > 0)
        {
            sCond = "(" + sCond.Substring(1, sCond.Length - 4) + ")";
            sSql = "select Distinct StoreNo, ";
            sSql += "CASE ";
            sSql += "WHEN PATINDEX('%[^0-9]%',DisplayNo) = 0 THEN CAST(DisplayNo as int) ";
            sSql += " ELSE ";
            sSql += " CASE ";
            sSql += "   WHEN LEN(SUBSTRING(DisplayNo, 1, PATINDEX('%[^0-9]%',DisplayNo) - 1)) = 0 THEN 99999999 ";
            sSql += " ELSE Cast(SUBSTRING(DisplayNo, 1, PATINDEX('%[^0-9]%',DisplayNo) - 1) as Int) ";
            sSql += " END ";
            sSql += " END NumericValue, ";
            sSql += " CASE ";
            sSql += " WHEN PATINDEX('%[^0-9]%',DisplayNo) = 0 THEN NULL ";
            sSql += " ELSE SUBSTRING(DisplayNo, PATINDEX('%[^0-9]%',DisplayNo), LEN(DisplayNo) - (PATINDEX('%[^0-9]%',DisplayNo) - 1)) ";
            sSql += " END AlphaNumericValue ";
        }
        if (iLoc == 1)
        {
            sSql += " from Stores s inner join Districts Dis ";
            sSql += " on s.District = Dis.District inner join Regions Reg ";
            sSql += " on Dis.Region = Reg.Region inner join Divisions Div ";
            sSql += " on Reg.Division = Div.Division";
            sSql += " where (" + sCond + ") ";
            sSql += " order by NumericValue,AlphaNumericValue";
        }
        else if (iLoc == 2)
        {
            sSql += " from Stores s inner join Districts Dis ";
            sSql += " on s.District = Dis.District inner join Regions Reg ";
            sSql += " on Dis.Region = Reg.Region";
            sSql += " where (" + sCond + ") ";
            sSql += " order by NumericValue,AlphaNumericValue";
        }
        else if (iLoc == 3)
        {
            sSql += " from Stores s inner join Districts Dis ";
            sSql += " on s.District = Dis.District";
            sSql += " where (" + sCond + ") ";
            sSql += " order by NumericValue,AlphaNumericValue";
        }
        else if (iLoc == 4)
        {
            sSql += " from Stores s ";
            sSql += " where (" + sCond + ")";
            sSql += " order by NumericValue,AlphaNumericValue";
        }
        if (sSql != "")
        {

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                cmd.CommandText = sSql;
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    stores.Add(sdr["StoreNo"].ToString());
                }

                sdr.Close();
                sdr.Dispose();

            }
            catch { }
            finally
            {
                con.Close();
            }
        }

        return stores;
    }
}
