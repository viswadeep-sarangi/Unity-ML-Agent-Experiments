using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ReadJointPositionsTXT : MonoBehaviour
{
    string _filePath;
    double placeholder_time, initialTime;

    Dictionary<string, Vector3> JointName_Vector3Pos;
    public Dictionary<double, Dictionary<string, Vector3>> Timestamp_JointName_Vector3Pos;

    public event Action<Dictionary<double, Dictionary<string, Vector3>>> ParsingComplete;

    private IEnumerator Start()
    {
        placeholder_time = -1; initialTime = -1;
        JointName_Vector3Pos = new Dictionary<string, Vector3>();
        Timestamp_JointName_Vector3Pos = new Dictionary<double, Dictionary<string, Vector3>>();

        yield return new WaitForSeconds(2f);

        Load_and_Parse_File();
    }

    private bool Load_and_Parse_File()
    {

        _filePath = EditorUtility.OpenFilePanel("Choose StroMoHab data file", "C://_Viswa//gender-classifier-using-lstm-tensorflow//Datasets//All York and Florida Data Combined//F10615321R 30-August-2017 Wednesday", "txt");

        // Handle any problems that might arise when reading the text
        try
        {
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            StreamReader theReader = new StreamReader(_filePath, Encoding.Default);
            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        // Do whatever you need to do with the text line, it's a string now
                        // In this example, I split it into arguments based on comma
                        // deliniators, then send that array to DoStuff()
                        string[] entries = line.Split(null); // null splits it by white space

                        if (entries.Length > 0)
                            parseLine(entries);
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();

                ParsingComplete?.Invoke(Timestamp_JointName_Vector3Pos); // Fire the parsing complete event
                return true;
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    private void parseLine(string[] data)
    {
        
        if (data.Length == 2 && data[0].Contains("NEW") && data[1].Contains("DATA"))
        {
            if(placeholder_time>0 && !Timestamp_JointName_Vector3Pos.ContainsKey(placeholder_time) && JointName_Vector3Pos!=null)
            {
                Timestamp_JointName_Vector3Pos.Add(placeholder_time, JointName_Vector3Pos);
            }
            JointName_Vector3Pos = new Dictionary<string, Vector3>();
        }
        else
        {
            double currentTime = parseDateTime(data[1]);
            if (initialTime<0) { initialTime = currentTime; }
            double timeDiff_in_milliseconds = currentTime - initialTime;
            placeholder_time = timeDiff_in_milliseconds;
           

            string current_joint = data[2];
            int scaleFactor = 100;
            Vector3 current_joint_position =
                new Vector3(
                              Convert.ToSingle(data[3]) * scaleFactor, // X coordinate
                              Convert.ToSingle(data[4]) * scaleFactor, // Y coordinate
                              Convert.ToSingle(data[5]) * scaleFactor  // Z coordinate
                           );
            
            JointName_Vector3Pos[current_joint] = current_joint_position;
        }
    }

    public double parseDateTime(string dateTime)
    {
        string[] HMS_milliSeconds = dateTime.Split('.');
        int miliSeconds = Convert.ToInt32(HMS_milliSeconds[1]);
        string[] HMS = HMS_milliSeconds[0].Split(':');
        int hours = Convert.ToInt32(HMS[0]);
        int minutes = Convert.ToInt32(HMS[1]);
        int seconds = Convert.ToInt32(HMS[2]);
        double finalTimeInMS = (hours * 60 * 60 * 1000) + (minutes * 60 * 1000) + (seconds * 1000) + miliSeconds;
        return finalTimeInMS;
    }
}
