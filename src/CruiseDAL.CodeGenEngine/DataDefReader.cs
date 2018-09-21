using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace FMSCORM.CodeGenEngine
{
    

    public class DataDictionaryReader
    {
        //constants 
        //for reading of TableSettings.csv
        public const string TABLE_COLLECTION_INDICATOR = "!";
        public const string META_DATA_PREFIX = "$";
        public enum TableSettingsColumn : int { Name = 0, DeleteMethod, TrackCreated, TrackModified, TrackRowVersion } ;
        public int TSettings_Col_Name = 0;
        public int TSettings_Col_DeleteMethod = 1;
        public int TSettings_Col_TrackCreated = 2;
        public int TSettings_Col_TrackModified = 3;
        public int TSettings_Col_TrackMergeState = 4;
        public int TSettings_Col_TrackRowVersion = 5; 

        public List<TableCollection> TableCollections { get; set; }
        public Dictionary<String, Table> AllTables { get; set; }

        public DataDictionaryReader()
        {
            this.TableCollections = new List<TableCollection>();
            this.AllTables = new Dictionary<string,Table>();

            this.Read();
        }

        //public DataDictionaryReader(string dataDictionaryPath, string tableSettingPath) : this()
        //{
        //    this.TableSettingsPath = tableSettingPath;
        //    this.DataDictionaryPath = dataDictionaryPath;
        //}

        public string DBVersion { get; set; }
        public string SchemaVersion { get; set; }
        public string MinimumCompatibleVersion { get; set; }


        protected StreamReader GetDataDictionaryReader()
        {
            Assembly assembley = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembley.GetManifestResourceStream("CruiseDAL.CodeGenEngine.DataDictionary.csv");
            return new System.IO.StreamReader(stream);
        }

        protected StreamReader GetTableSettingReader()
        {
            Assembly assembley = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembley.GetManifestResourceStream("CruiseDAL.CodeGenEngine.TableSettings.csv");
            return new System.IO.StreamReader(stream);

        }
        
        public void ReadDataDictionary()
        {
            Table.TableList = new List<Table>();//am i haveing trouble with this static field?
            using(StreamReader reader = this.GetDataDictionaryReader())
            {
                reader.ReadLine(); //skip header
                
                String[] row = reader.ReadLine().Split(',');
                Table currentTable = new Table();
                currentTable.ReadLine(row);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    row = line.Split(',');
                    currentTable = currentTable.ReadLine(row);
                }

                foreach(Table t in Table.TableList)
                {
                    this.AllTables.Add(t.Name, t);
                }
            }
        }

        public void ReadTableSettings()
        {
            using(StreamReader reader = this.GetTableSettingReader())
            {
                TableCollection currentCollection = null;

                reader.ReadLine(); //skip header
                
                

                
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    String[] row = line.Split(',');
                    for (int i = 0; i < row.Length; i++)
                    {
                        row[i] = row[i].Trim('"');//trim quotes 
                        row[i] = row[i].Trim();//trun whitespace
                    }

                    if(String.IsNullOrEmpty(row[0]))
                    { continue; }

                    string sName = string.Empty;
                    string sDelMeth = string.Empty;
                    string sTracCreated = string.Empty;
                    string sTracMod = string.Empty;
                    string sTrackMerge = string.Empty;
                    string sTrackRowVersion = string.Empty;
                    try
                    {
                        sName = row[TSettings_Col_Name];
                        sDelMeth = row[TSettings_Col_DeleteMethod];
                        sTracCreated = row[TSettings_Col_TrackCreated];
                        sTracMod = row[TSettings_Col_TrackModified];
                        sTrackMerge = row[TSettings_Col_TrackMergeState];
                        sTrackRowVersion = row[TSettings_Col_TrackRowVersion];
                    }
                    catch { }

                    if (sName.StartsWith(TABLE_COLLECTION_INDICATOR))
                    {
                        currentCollection = new TableCollection()
                        {
                            Name = sName.Substring(1),
                            DeleteMethod = this.ParseDeleteMethod(sDelMeth),
                            TrackCreated = this.ParseBool(sTracCreated),
                            TrackModified = this.ParseBool(sTracMod),
                            TrackMergeState = this.ParseBool(sTrackMerge),
                            TrackRowVersion = this.ParseBool(sTrackRowVersion)
                        };
                        this.TableCollections.Add(currentCollection);
                        continue;
                    }
                    else if(sName.StartsWith(META_DATA_PREFIX))
                    {
                        string[] s = sName.Split('=');
                        switch (s[0])
                        {
                            case "$SchemaVersion":
                                {
                                    this.SchemaVersion = s[1];
                                    break;
                                }
                            case "$MinimumCompatibleVersion":
                                {
                                    this.MinimumCompatibleVersion = s[1];
                                    break;
                                }
                            case "$DBVersion":
                                {
                                    this.DBVersion = s[1];
                                    break;
                                }
                        }
                        continue;

                    }
                    else if (currentCollection == null)
                    {
                        currentCollection = new TableCollection();
                        this.TableCollections.Add(currentCollection);
                    }

                    Table curTable = this.AllTables[sName];
                    
                    curTable.DeleteMethod = this.ParseDeleteMethod(sDelMeth);
                    
                    //track created and track modifeid can either be explicitly marked true or false, or be blank
                    //blank will be read as null and will defer track created and track modified to the table collection
                    bool? trackCreated = this.ParseNBool(sTracCreated);
                    bool? trackModified = this.ParseNBool(sTracMod);
                    bool? trackMerge = this.ParseNBool(sTrackMerge);
                    bool? trackRowVersion = this.ParseNBool(sTrackRowVersion);
                    if(trackCreated != null)
                    {
                        curTable.TrackCreated = trackCreated.Value;
                    }
                    if(trackModified != null)
                    {
                        curTable.TrackModified = trackModified.Value;
                    }
                    if (trackMerge != null)
                    {
                        curTable.TrackMergeState = trackMerge.Value;
                    }
                    if (trackRowVersion != null)
                    {
                        curTable.TrackRowVersion = trackRowVersion.Value;
                    }

                    curTable.TableCollection = currentCollection;
                    currentCollection.Add(curTable);
                }


            }
        }

        public void Read()
        {
            this.ReadDataDictionary();
            this.ReadTableSettings();
        }

        public DeleteMethodType ParseDeleteMethod(string str)
        {
            if(string.IsNullOrEmpty(str)) { return DeleteMethodType.Unknown; }
            if(!Enum.IsDefined(typeof(DeleteMethodType), str)) { return DeleteMethodType.Unknown; }
            return (DeleteMethodType)Enum.Parse(typeof(DeleteMethodType), str);
        }

        public bool? ParseNBool(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return null;
            }
            return ParseBool(str);
        }

        public bool ParseBool(String str)
        {
            switch (str.ToUpper())
            {
                case "TRUE":
                case "1":
                case "X":
                    {
                        return true;
                    }
                case "FALSE":
                case "0":
                case "-":
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
