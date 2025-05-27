using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataSaves<T> where T : class
{
    public static DataSavesObject<T> instance => (_instance ?? (_instance = new DataSavesObject<T>()));
    private static DataSavesObject<T> _instance;

    public static T Data => instance.Data;

    public static void Save() => instance.Save();
    public static void Load() => instance.Load();
    public static void Reset() => instance.Reset();

}


public class DataSavesObject<T> where T : class
{
    public string filesaveName { get; private set; }
    private SerializationBinder serializationBinder { get; set; }


    public T Data { get { return _data; } }
    private T _data = null;

    public DataSavesObject(string dataname = null)
    {
        this.filesaveName = (dataname ?? $"data_{typeof(T).Name}") + ".dat";
    }

    public virtual void Save() => DataSavesStatic.SaveSerrialize(filesaveName, ref _data);

    public virtual void Load()
    {
        if (!DataSavesStatic.LoadSerrialize(filesaveName, ref _data, serializationBinder))
        {
            Reset();
        }
    }

    public virtual void Reset()
    {
        _data = Activator.CreateInstance<T>();
        Save();
    }

    public void SetBinder(SerializationBinder binder)
    {
        this.serializationBinder = binder;
    }


}


public static class DataSavesStatic
{
    [System.Serializable] public class NoData { }

    public static event Action OnSavingError;

    public static bool SaveSerrialize<T>(string filename, ref T variable) where T : class
    {
        if (variable != null)
        {
            Stream fileStream = null;
            try
            {

                //Debug.Log("Save " + typeof(T).Name + "...");


                BinaryFormatter bf = new BinaryFormatter();
                fileStream = File.Open(Application.persistentDataPath + "/" + filename, FileMode.OpenOrCreate);

                //throw new IOException("Imitation Disk End exception");

                bf.Serialize(fileStream, variable);

                return true;

            }
            catch (IOException ex)
            {
                Debug.LogError("CATCH SAVE EXCEPTION on " + filename + ": " + ex.Message);

                OnSavingError?.Invoke();
            }
            finally
            {
                if (fileStream != null) fileStream.Close();
            }
        }

        return false;
    }

    public static bool LoadSerrialize<T>(string filename, ref T variable, SerializationBinder binder = null) where T : class
    {

        if (File.Exists(Application.persistentDataPath + "/" + filename))
        {
            FileStream file = null;
            try
            {
                Debug.Log("Load " + typeof(T).Name + "...");

                BinaryFormatter bf = new BinaryFormatter();
                if (binder != null)
                    bf.Binder = binder;

                file = File.Open(Application.persistentDataPath + "/" + filename, FileMode.Open);

                variable = (T)bf.Deserialize(file);

                Debug.Log("Loading success");

                // TODO: »справить потерю точности модели на длительном(мес€ц+) промежутке игры. 
                // ‘лоаты не подход€т, либо оптимизировать базовое врем€.
                // следующий код показывает проблему:
                //(variable as SavesData.GameSavesData).timestamp = (variable as SavesData.GameSavesData).timestamp - TimeSpan.TicksPerDay * 30;

                return true;

            }
            catch (EndOfStreamException ex)
            {

                Debug.Log("Can't load saves " + filename);
                Debug.Log(ex.Message);
            }
            catch (SerializationException ex)
            {
                Debug.Log("Can't load saves " + filename);
                Debug.Log(ex.Message);
            }
            finally
            {
                if (file != null) file.Close();

            }

        }

        return false;
    }



}

