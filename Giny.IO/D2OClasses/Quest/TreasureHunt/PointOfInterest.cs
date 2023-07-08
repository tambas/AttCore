using System;
using Giny.Core.IO.Interfaces;
using Giny.IO.D2O;
using Giny.IO.D2OTypes;
using System.Collections.Generic;

namespace Giny.IO.D2OClasses
{    [D2OClass("PointOfInterest", "")]
    public class PointOfInterest : IDataObject , IIndexedData
    {        public const string MODULE = "PointOfInterest";

        public int Id => (int)id;

        public uint id;
        public uint nameId;
        public uint categoryId;

        [D2OIgnore]
        public uint Id_
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        [D2OIgnore]
        public uint NameId
        {
            get
            {
                return nameId;
            }
            set
            {
                nameId = value;
            }
        }
        [D2OIgnore]
        public uint CategoryId
        {
            get
            {
                return categoryId;
            }
            set
            {
                categoryId = value;
            }
        }

    }}
