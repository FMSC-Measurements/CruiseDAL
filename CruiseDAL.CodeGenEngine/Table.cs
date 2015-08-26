using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.CodeGenEngine
{
    public class Table
    {
        public static int Table_Ord = 1;
        public static int Column_Ord = 2;
        public static int Type_Ord = 3;
        public static int PK_Ord = 4;
        public static int AutoI_Ord = 5;
        public static int TPK_Ord = 6;
        public static int Unique_Ord = 7;
        public static int FK_Ord = 8;
        public static int Ref_Ord = 9;
        public static int Req_Ord = 10;
        public static int Default_Ord = 11;
        public static int NotNull_Ord = 12;
        public static int Min_Ord = 13;
        public static int Max_Ord = 14;
        public static int Values_Ord = 15;
        public static int ErrorMessage_Ord = 16;
        public static int Description_Ord = 17;
        public static int Depreciated_Ord = 19;
        public static int DontTrackChanges_Ord = 20;
        //public static readonly String[] DEPRECIATED_FIELDNAMES = new String[]{ "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate" };
        public static readonly string[] UTILITY_FIELDS = new string[] { "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate" };

        public static List<Table> TableList = new List<Table>();//global

        private bool? _trackCreated;
        private bool? _trackModified;
        private bool? _trackMergeState;
        private bool? _trackRowVersion; 
       
        public Table()
        {
            this.IsDepreciated = true;
        }

        public Table(String tableName) : this()
        {
            Name = tableName;
        }

        public TableCollection TableCollection { get; set; }//parent table collection

        public DeleteMethodType DeleteMethod { get; set; }
        public bool TrackCreated 
        { 
            get
            {
                if(_trackCreated == null && TableCollection != null)
                {
                    return TableCollection.TrackCreated;
                }
                else if(_trackCreated == null)
                {
                    return false;
                }
                else
                {
                    return _trackCreated.Value;
                }
            }
            set
            {
                _trackCreated = value;
            }
        }
        public bool TrackModified 
        {
            get
            {
                if (_trackModified == null && this.TableCollection != null)
                {
                    return TableCollection.TrackModified;
                }
                else if (_trackModified == null)
                {
                    return false;
                }
                else
                {
                    return _trackModified.Value;
                }
            }
            set
            {
                _trackModified = value;
            }
        }

        public bool TrackRowVersion
        {
            get
            {
                if (_trackRowVersion == null && this.TableCollection != null)
                {
                    return TableCollection.TrackRowVersion;
                }
                else if (_trackRowVersion == null)
                {
                    return false;
                }
                else
                {
                    return _trackRowVersion.Value;
                }
            }
            set
            {
                _trackRowVersion = value;
            }
        }


        public bool TrackMergeState
        {
            get
            {
                if (_trackMergeState == null && this.TableCollection != null)
                {
                    return TableCollection.TrackMergeState;
                }
                else if (_trackMergeState == null)
                {
                    return false;
                }
                else
                {
                    return _trackMergeState.Value;
                }
            }
            set
            { _trackMergeState = value; }
        }

        public bool IsDepreciated { get; set; }

        public bool HasTransitionalPK { get; set; }
        

        public String Name { get; set; }
        public List<Field> Fields = new List<Field>();
        public List<Field> DataFields = new List<Field>();
        public List<Field> UniqueFields = new List<Field>();
        public List<Field> PKFields = new List<Field>();
        public List<Field> FKFields = new List<Field>();
        public List<Field> AuditFields = new List<Field>();
        public List<Field> TrackedFields = new List<Field>();
        //public Field ModifiedDate { get; set; }
        //public bool hasCreatedBy = false;        


        #region parse methods
        public Table ReadLine(String[] row)
        {
            string tName = row[Table_Ord].Trim();
            if (Name == null)
            {
                Name = tName;
                TableList.Add(this);
            }
            else if (Name != tName)
            {
                Table newTable = new Table(tName);
                newTable.ReadLine(row);
                TableList.Add(newTable);
                return newTable;
            }
            //if(IsMarked(row[Depreciated_Ord])) { return this; }

            //if (DEPRECIATED_FIELDNAMES.Contains(row[Column_Ord])) { return this; } 

            Field newField = new Field
            {
                Name = row[Column_Ord],
                Type = row[Type_Ord],
                IsPK = IsMarked(row[PK_Ord]),
                IsAutoI = IsMarked(row[AutoI_Ord]),
                IsTransitionalPK = IsMarked(row[TPK_Ord]),
                IsUnique = IsMarked(row[Unique_Ord]),
                IsFK = IsMarked(row[FK_Ord]),
                IsReq = IsMarked(row[Req_Ord]),
                Default = row[Default_Ord].Replace("\"",String.Empty),
                NotNull = IsMarked(row[NotNull_Ord]),
                Min = (!String.IsNullOrEmpty(row[Min_Ord])) ? (float?)Convert.ToSingle(row[Min_Ord]) : null,
                Max = (!String.IsNullOrEmpty(row[Max_Ord])) ? (float?)Convert.ToSingle(row[Max_Ord]) : null,
                Values = row[Values_Ord],
                ErrorMessage = row[ErrorMessage_Ord],
                Description = row[Description_Ord],
                IsDepreciated = IsMarked(row[Depreciated_Ord]),
                DontTrackChanges = IsMarked(row[DontTrackChanges_Ord])
            };

            if (newField.IsTransitionalPK)
            {
                this.HasTransitionalPK = true;
            }

            if (newField.IsPK == true) 
            { 
                PKFields.Add(newField);
                
            }
            else if (newField.IsFK == true) 
            { 
                newField.Ref = row[Ref_Ord];
                FKFields.Add(newField); 
            }
            else { DataFields.Add(newField); }
            

            if(UTILITY_FIELDS.Contains(newField.Name))
            {
                newField.DontTrackChanges = true;
            }


            if (newField.IsUnique == true) { UniqueFields.Add(newField); }
            Fields.Add(newField);

            if (newField.Min != null || newField.Max != null || !String.IsNullOrEmpty(newField.Values))
            {
                AuditFields.Add(newField);
            }

            //TODO comment out 
            //? I guess if a table has no non depreciated fields then it is depreciated, 
            if (newField.IsDepreciated == false)
            {
                this.IsDepreciated = false;
            }

            if (!newField.DontTrackChanges)
            {
                this.TrackedFields.Add(newField);
            }
            return this;
        }

        private bool IsMarked(String s)
        {
            s = s.Trim();
            return s == "X";
        }

       
        #endregion

        #region sql gen methods
        public string ListTrackedFields()
        {
            string[] fieldNames = (from f in this.TrackedFields
                                   select f.Name).ToArray();
            return string.Join(", ", fieldNames); 
        }

        #endregion
    }
}
