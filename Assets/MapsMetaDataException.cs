using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapsMetaDataException : System.Exception
{
    public MapsMetaDataException()
    {

    }

    public MapsMetaDataException(string line) : base(string.Format("Illegal Types in the metaData file", line)){

    }
}
