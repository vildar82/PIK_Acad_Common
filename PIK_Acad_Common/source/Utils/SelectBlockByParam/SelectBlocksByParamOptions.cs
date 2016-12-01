using AcadLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_Acad_Common.Utils.SelectBlockByParam
{
    /// <summary>
    /// Настройки - блоки и параметры
    /// </summary>
    public class SelectBlocksByParamOptions
    {
        private static string plugin = "SelectBlocksByParam";
        private static string file = AcadLib.IO.Path.GetUserPluginFile(plugin, "SelectBlocksByParamOptions.xml");

        public List<SelectBlockParam> BlockParams { get; set; }

        public SelectBlocksByParamOptions()
        {
            BlockParams = new List<SelectBlockParam>();
        }

        public List<string> Find(string blName)
        {
            List<string> resParams = null;
            var selBlParam = BlockParams.Find(b => b.BlockName.Equals(blName, StringComparison.OrdinalIgnoreCase));
            if (selBlParam != null)
            {
                resParams = selBlParam.SelectedParams;
            }
            return resParams;
        }

        public void AddBlockSelParams (string blName, List<string> selParams)
        {
            var selBlParam = BlockParams.Find(b => b.BlockName.Equals(blName, StringComparison.OrdinalIgnoreCase));
            if (selBlParam == null)
            {
                selBlParam = new SelectBlockParam(blName, selParams);
                BlockParams.Add(selBlParam);
            }
            else
            {
                selBlParam.SelectedParams = selParams;
            }
        }

        public static SelectBlocksByParamOptions Load()
        {
            if (File.Exists(file))
            {
                var ser = new AcadLib.Files.SerializerXml(file);
                try
                {
                    var res = ser.DeserializeXmlFile<SelectBlocksByParamOptions>();
                    return res;
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"SelectBlocksByParamOptions.Load - {file}");
                }
            }
            return new SelectBlocksByParamOptions();
        }

        public void Save()
        {
            var ser = new AcadLib.Files.SerializerXml(file);
            try
            {
                ser.SerializeList<SelectBlocksByParamOptions>(this);                
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"SelectBlocksByParamOptions.Save - {file}");
            }            
        }
    }

    public class SelectBlockParam
    {
        public SelectBlockParam()
        {

        }
        public SelectBlockParam(string blName, List<string> selectedParams)
        {
            BlockName = blName;
            SelectedParams = selectedParams;
        }

        public string BlockName { get; set; }
        public List<string> SelectedParams { get; set; }        
    }
}
