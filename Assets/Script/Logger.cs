using UnityEngine;
using System.Collections;
using System.Text;

public class Logger
{
    private StringBuilder sb;

    public Logger()
    {
        sb = new StringBuilder();
    }

    public void Print()
    {
        Debug.Log(sb.ToString());
    }

    public void Log(string log)
    {
        sb.Append(log+"\n");
    }
}
