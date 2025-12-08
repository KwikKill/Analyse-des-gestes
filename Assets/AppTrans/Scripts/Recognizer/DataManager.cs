using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Recognizer.EvaluationTools;
using UnityEngine;

namespace Recognizer
{
    public class DataManager
    {
        private static DataManager INSTANCE = new DataManager();
        private static string _pathRaw;

        public static DataManager GetInstance()
        {
            _pathRaw = "./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathRaw;
            return INSTANCE;
        }

        /// <summary>
        /// Donne toutes les données enregistrées
        /// </summary>
        public Dictionary<string, List<GestureData>> GetDataClasses()
        {
            Dictionary<string, List<GestureData>> data = new Dictionary<string, List<GestureData>>();

            string path = System.IO.Path.GetFullPath(_pathRaw);

            DirectoryInfo d = new DirectoryInfo(@path); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                string[] dataLines = File.ReadAllLines(file.FullName);
                string first = dataLines.First();
                if (!first.Contains("="))
                    continue;
                string classe = first.Substring(first.IndexOf('=') + 1, first.IndexOf('>') - first.IndexOf('=') - 1);

                if (!data.ContainsKey(classe))
                {
                    data.Add(classe, new List<GestureData>());
                }

                data[classe].Add(new GestureData(file.FullName, dataLines, classe, file.Name));
            }

            return data;
        }
        
        /// <summary>
        /// Donne toutes les données enregistrées : + opti
        /// </summary>
        public Dictionary<string, Func<List<GestureData>>> GetDataClassesLAZYAndHeaderDergOnly()
        {
            Dictionary<string, Func<List<GestureData>>> data = new Dictionary<string, Func<List<GestureData>>>();


            List<string> classeName = GetAllDataClasseName();

            foreach (string classe in classeName)
            {
                Func<List<GestureData>> a = () =>GetListOfGestureDataForClasse(classe);
                data.Add(classe,a);
            }

            return data;

        }

        private List<GestureData> GetListOfGestureDataForClasse(string classe)
        {
            string path = System.IO.Path.GetFullPath(_pathRaw);
            List<GestureData> data = new List<GestureData>();
            
            DirectoryInfo d = new DirectoryInfo(@path); 
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                IEnumerable<string> dataLines = File.ReadLines(file.FullName);
                string s = dataLines.First();
                if (!s.Contains("="))
                    continue;
                
                string classeNameInFile = s.Substring(s.IndexOf('=') + 1, s.IndexOf('>') - s.IndexOf('=') - 1);

                if (!file.Name.Contains(classe))
                    continue;

                if (classeNameInFile != classe && classe!="Sequence")
                    continue;

                if (classe == "Sequence")
                {
                    Action<StringBuilder> lambda = (StringBuilder strB) => ReadOneInString(strB,dataLines);
                    List<string> miniClasses = classeNameInFile.Split(new string[] {"__"}, StringSplitOptions.None).Where(st=>st!="" && st!=" ").ToList();
                    data.Add(new GestureData(file.FullName,lambda, classe, file.Name,miniClasses.ToList()));
                }
                else
                {
                    Action<StringBuilder> lambda = (StringBuilder strB) => ReadOneInString(strB,dataLines);
                    data.Add(new GestureData(file.FullName,lambda, classe, file.Name));
                }


            }
            return data;

        }

        private void ReadOneInString(StringBuilder stringBuilder,IEnumerable<string> data)
        {
            foreach (string s in data)
            {
                stringBuilder.Append(s).AppendLine();
            }
        }

        /// <summary>
        /// Donne GestureData (random) d'un geste choisi
        /// </summary>
        /// <param name="classe"> Le nom du geste dont on veut la GestureData </param>
        /// <returns></returns>
        public GestureData GetGestureDataFromClassName(string classe)
        {

            string path = System.IO.Path.GetFullPath(_pathRaw);

            DirectoryInfo d = new DirectoryInfo(@path); 
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            
            //On shuffle les fichiers :
            //-Pas le même geste à chaque fois
            //-Plus rapide en moyenne (pas tous les exemples d'un même geste à la suite)
            List<FileInfo> filesList=Files.Shuffle();
            
            foreach (FileInfo file in filesList)
            {
                IEnumerable<string> dataLines = File.ReadLines(file.FullName);
                string first = dataLines.First();
                if (!first.Contains("="))
                    continue;
                string classeName = first.Substring(first.IndexOf('=') + 1, first.IndexOf('>') - first.IndexOf('=') - 1);

                if (classeName == classe || (classe=="Sequence" && file.Name.Contains("Sequence")))
                {
                    return new GestureData(file.FullName, File.ReadAllLines(file.FullName), classe, file.Name);
                }
               
            }

            return null;
        }


        /// <summary>
        /// Donne une texture représentant la projection en 2D des trajectoires du geste passé en paramètre
        /// </summary>
        /// <param name="gestureData"></param>
        /// <returns></returns>
        public Texture2D GetImageFromDataGesture(GestureData gestureData)
        {

            if ((!Directory.Exists(Application.dataPath + "/../gestePicture")) || (!File.Exists(Application.dataPath + "/../gestePicture/" + gestureData.DataName + ".png")))
                ImageGenerator.saveImage(gestureData.Path, gestureData.DataName,1000, 1000);
            
            byte[] fileData = File.ReadAllBytes(Application.dataPath + "/../gestePicture/"+gestureData.DataName+".png");
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            return tex;
        }

        /// <summary>
        /// remove a gesture data from all data (action irreversible), utilise le path
        /// </summary>
        /// <param name="data"></param>
        public void Delete(GestureData data)
        {
            File.Delete(data.Path);
        }

        /// <summary>
        /// Give all avaible classes depending of files
        /// </summary>
        public List<string> GetAllDataClasseName()
        {
            try
            {
                string filePath =
                    System.IO.Path.GetFullPath(
                        _pathRaw+"/HeaderDERG.txt");

                string[] lines = System.IO.File.ReadAllLines(filePath);
                int i;
                for (i = 0; i < lines.Length; i++)
                    if (lines[i].StartsWith("#"))
                        break;
                List<string> classes = lines[i].Split('#').Where(x => x.Length != 0).ToList();
                return classes;
            }
            catch (Exception)
            {
                Exception ex = new Exception("You need to create a file HeaderDERG.txt into the RAW folder and specify the available gestures separated with '#'");
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void AddDataClassNameInHeaderDerg(string classe)
        {
            try
            {
                string filePath =
                    System.IO.Path.GetFullPath(
                        _pathRaw+"/HeaderDERG.txt");

                string[] lines = System.IO.File.ReadAllLines(filePath);
                int i;
                for (i = 0; i < lines.Length; i++)
                    if (lines[i].StartsWith("#"))
                        break;

                lines[i] = lines[i] + "#" + classe;
                File.WriteAllLines(filePath,lines);
            }
            catch (Exception)
            {
                Exception ex = new Exception("You need to create a file HeaderDERG.txt into the RAW folder and specify the available gestures separated with '#'");
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Donne le lieu d'enregistrement des données
        /// </summary>
        /// <returns></returns>
        public string GetPathForRecording()
        {
           return Path.GetFullPath(_pathRaw+"/");
        }

        /// <summary>
        /// Add a data, utilise l'attribut data (pas le path)
        /// </summary>
        /// <param name="data"></param>
        public bool AddData(GestureData data)
        {
            string path = System.IO.Path.GetFullPath(_pathRaw+"/" +
                                                     data.DataName );
        
            if (System.IO.File.Exists(path))
                 return false;

            data.Path = path;
            File.WriteAllText(path, data.ExtractData());

            if (!GetAllDataClasseName().Contains(data.Classe))
                AddDataClassNameInHeaderDerg(data.Classe);
            
            return true;
        }
    }
}