﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_DEVICE =
@"CREATE TABLE Device ( 
    DeviceID TEXT NOT NULL COLLATE NOCASE, 
    Name TEXT, 

    UNIQUE (DeviceID)
);
";

    }
}