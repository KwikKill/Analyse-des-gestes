using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recognizer
{
    public class GestureData
    {
        private readonly List<string> _labelsRecorded;
        public string Path { get; set; }
        private string[] Data  { get; set; }
        private string DataFullOpti  { get; set; }
        public string Classe  { get; set; }
        public string DataName  { get; set; }
        private bool _isDataOneString;
        private  Action<StringBuilder>  _promiseData;
        private  string _promiseDataReaded;

        public GestureData(string path, string[] data, string classe,string dataName)
        {
            _isDataOneString = false;
            Path = path;
            Data = data;
            Classe = classe;
            DataName = dataName;
        }
        public GestureData(string path, Action<StringBuilder> data, string classe,string dataName)
        {
            _isDataOneString = false;
            Path = path;
            _promiseData = data;
            Classe = classe;
            DataName = dataName;
        }
        
        public GestureData(string path, Action<StringBuilder> data, string classe,string dataName, List<string> labelsRecorded):this(path,data,classe,dataName)
        {
            _labelsRecorded = labelsRecorded;
        }
        
        public GestureData(string path, string data, string classe,string dataName)
        {
            _isDataOneString = true;
            Path = path;
            DataFullOpti = data;
            Classe = classe;
            DataName = dataName;
        }


        public GestureData(string classe,string dataName)
        {
            Classe = classe;
            DataName = dataName;
        }

        public GestureData(string classe, string toString, string getClassName, string fileName, List<string> labelsRecorded):this(classe, toString, getClassName,fileName)
        {
            _labelsRecorded = labelsRecorded;
        }

        public List<string> ListsLabelsForSequence()
        {
            return _labelsRecorded;
        }

        public string ExtractData()
        {
            if (!_isDataOneString)
            {
                if (_promiseData!=null)
                {
                    if (_promiseDataReaded != null)//si déjà lu
                        return _promiseDataReaded;
                    StringBuilder s = new StringBuilder();
                    _promiseData(s);
                    _promiseDataReaded = s.ToString();
                    return _promiseDataReaded;
                }
                else
                {
                    string str = "";
                    if (_labelsRecorded != null)
                    {
                        str = "<class="+_labelsRecorded.Aggregate("",(x,y)=>x+"__"+y)+">\n";
                    }else
                        str = "<class="+Classe+">\n";
                    str+=String.Join("\n",Data);
                    str += "</class="+Classe+">";
                    return str;
                }
             
            }
            if (_labelsRecorded != null)
            {
                string classes = _labelsRecorded.Aggregate("", (x, y) => x + "__" + y) + ">\n";
                return "<class="+classes+DataFullOpti+"</class="+Classe+">";
            }else
                return "<class="+Classe+">\n"+DataFullOpti+"</class="+Classe+">";
        }
    }
}