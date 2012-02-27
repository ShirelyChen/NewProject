using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TotalHours
/// List of StoreTotal
/// 4--Store,3--District,2--Region,1--Division
/// </summary>
public class DataTotal<T>
{
    private List<StoreTotal<T>> storeTotalList;
    public List<StoreTotal<T>> StoreTotalList { get { return storeTotalList;} }

    public DataTotal(string sDB,DataType dataType, int locationType, string[] locationID, DateTime startDate, DateTime endDate,Calculator<T> calculator)
	{
        StoresFromDB storesFromDB = new StoresFromDB(sDB);
        
        if ((locationType == 4) && (locationID.Length == 1) && (locationID[0] == "0"))
        {
            HolidaysFromDB holidayFromDB = new HolidaysFromDB(sDB);
            List<StoreHoliday> storeHolidayList = holidayFromDB.GetAllStoresHolidayList();
            PeriodsFromDB periodFromDB = new PeriodsFromDB(sDB);
            List<StorePeriods> storePeriodList = periodFromDB.GetAllStoresHourList();
            DataFromDatabase<T> dataFromDB = new DataFromDatabase<T>(sDB);
            List<StoreDailyDataList<T>> allStoresDailyData = dataFromDB.GetAllStoreHourlyData( startDate, endDate, dataType);
            storeTotalList = new List<StoreTotal<T>>(allStoresDailyData.Count);
            foreach (StoreDailyDataList<T> storeDailyDataList in allStoresDailyData)
            {
                StoreTotal<T> storeTotal = new StoreTotal<T>(storeDailyDataList.StoreID);
                StorePeriods storePeriod = new StorePeriods(storeDailyDataList.StoreID);
                storePeriod.GetStorePeriodByStoreID(storePeriodList, storeDailyDataList.StoreID);
                StoreHoliday storeHoliday = new StoreHoliday(storeDailyDataList.StoreID);
                storeHoliday.GetStoreHolidayByStoreID(storeHolidayList, storeDailyDataList.StoreID);
                storeTotal = storeDailyDataList.GetStoreTotalByOpenAndCloseHour(storePeriod, storeHoliday, calculator);
                storeTotalList.Add(storeTotal);
            }
        }
        else if ((locationType == 4) && (locationID.Length < 5))
        {
            Stores store = storesFromDB.GetStoreList(locationID, locationType);
            storeTotalList = new List<StoreTotal<T>>(store.StoreList.Count);
            foreach (string strStoreID in store.StoreList)
            {
                HolidaysFromDB holidayFromDB = new HolidaysFromDB(sDB);
                StoreHoliday storeHoliday = holidayFromDB.GetHolidayListByStore(strStoreID);
                PeriodsFromDB periodFromDB = new PeriodsFromDB(sDB);
                StorePeriods storePeriod = periodFromDB.GetHourListByStore(strStoreID);
                DataFromDatabase<T> dataFromDB = new DataFromDatabase<T>(sDB);
                StoreDailyDataList<T> storeDailyDataList = dataFromDB.GetHourlyDataByStore(strStoreID, startDate, endDate, dataType);
                StoreTotal<T> storeTotal = new StoreTotal<T>(strStoreID);
                storeTotal = storeDailyDataList.GetStoreTotalByOpenAndCloseHour(storePeriod, storeHoliday, calculator);
                storeTotalList.Add(storeTotal);
            }
        }
        else
        {
            HolidaysFromDB holidayFromDB = new HolidaysFromDB(sDB);
            List<StoreHoliday> storeHolidayList = holidayFromDB.GetAllStoresHolidayList();
            PeriodsFromDB periodFromDB = new PeriodsFromDB(sDB);
            List<StorePeriods> storePeriodList = periodFromDB.GetAllStoresHourList();
            DataFromDatabase<T> dataFromDB = new DataFromDatabase<T>(sDB);
            List<StoreDailyDataList<T>> allStoresDailyData = dataFromDB.GetHourlyDataByLocation(locationType, locationID, startDate, endDate, dataType);
            storeTotalList = new List<StoreTotal<T>>(allStoresDailyData.Count);
            foreach (StoreDailyDataList<T> storeDailyDataList in allStoresDailyData)
            {
                StoreTotal<T> storeTotal = new StoreTotal<T>(storeDailyDataList.StoreID);
                StorePeriods storePeriod = new StorePeriods(storeDailyDataList.StoreID);
                storePeriod.GetStorePeriodByStoreID(storePeriodList, storeDailyDataList.StoreID);
                StoreHoliday storeHoliday = new StoreHoliday(storeDailyDataList.StoreID);
                storeHoliday.GetStoreHolidayByStoreID(storeHolidayList, storeDailyDataList.StoreID);
                storeTotal = storeDailyDataList.GetStoreTotalByOpenAndCloseHour(storePeriod, storeHoliday, calculator);
                storeTotalList.Add(storeTotal);
            }
        }
	}
    
    

}






