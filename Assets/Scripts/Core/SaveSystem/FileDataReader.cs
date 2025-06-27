using System;
using System.IO;
using UnityEngine;

namespace Core.SaveSystem
{
    public class FileDataReader
    {
        private string dataDirPath = "";
        private string dataFileName = "";

        public FileDataReader(string dataDirPath, string dataFileName)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
        }

        public GameData Load(string profileId)
        {
            // base case - if the profileId is null, return right away
            if (profileId == null)
            {
                return null;
            }

            // use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                // load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }

            return loadedData;
        }

        public void Save(GameData data, string profileId)
        {
            // base case - if the profileId is null, return right away
            if (profileId == null)
            {
                return;
            }

            // use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            try
            {
                // create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // serialize the C# game data object into Json
                string dataToStore = JsonUtility.ToJson(data, true);

                // write the serialized data to the file
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
    }
}
