using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
namespace epona
{
    class LocalizationParser
    {
        public static Dictionary<string, Localization> Load(TextAsset asset)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Xml2CSharp.Workbook));
            Xml2CSharp.Workbook book = serializer.Deserialize(new StringReader(asset.text)) as Xml2CSharp.Workbook;
            return Load(book);
        }

        public static Dictionary<string, Localization> Load(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open);
                Dictionary<string, Localization> result = Load(stream);
                stream.Close();
                return result;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Exception loading lang file: " + e);
                if (stream != null)
                {
                    stream.Close();
                }
                return null;
            }

         }

        public static Dictionary<string, Localization> Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Xml2CSharp.Workbook));
            Xml2CSharp.Workbook book = serializer.Deserialize(stream) as Xml2CSharp.Workbook;
            return Load(book);
        }

        public static Dictionary<string, Localization> Load(Xml2CSharp.Workbook book)
        {
            try
            {

                Dictionary<string, Localization> localizations = new Dictionary<string, Localization>();

                foreach (Xml2CSharp.Worksheet sheet in book.Worksheet)
                {
                    Dictionary<int, Localization> localizationsByColumn = new Dictionary<int, Localization>();
                    int rowId = 0;
                    foreach (Xml2CSharp.Row row in sheet.Table.Row)
                    {
                        if (rowId == 0)
                        {
                            int cellId = 0;
                            foreach (Xml2CSharp.Cell cell in row.Cell)
                            {
                                if (cellId == 0)
                                {
                                    //This is the key column, ignore it
                                }
                                else
                                {
                                    string languageCode = cell.Data.Text;
                                    Localization loc;
                                    if (localizations.ContainsKey(languageCode))
                                    {
                                        loc = localizations[languageCode];
                                    }
                                    else
                                    {
                                        loc = new Localization();
                                        localizations.Add(languageCode, loc);
                                    }

                                    localizationsByColumn.Add(cellId, loc);
                                }

                                cellId++;
                            }
                        }
                        else
                        {
                            int cellId = 0;
                            string key = "";
                            foreach (Xml2CSharp.Cell cell in row.Cell)
                            {
                                if (cell != null && cell.Data != null)
                                {
                                    if (cellId == 0)
                                    {
                                        //The key field

                                        key = cell.Data.Text;

                                    }
                                    else
                                    {
                                        Localization loc = localizationsByColumn[cellId];
                                        string value = cell.Data.Text;
                                        loc.Set(new LocalizationId(key), value);
                                    }
                                }
                                cellId++;
                            }
                        }
                        rowId++;
                    }
                }

                return localizations;

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Exception loading lang file: " + e);
                return null;
            }
        }
    }
}