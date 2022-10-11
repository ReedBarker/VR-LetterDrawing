using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecordPosition : MonoBehaviour
{
    const int arrayLength = 150;
    string filepath = "DATA1.data";
    int i = 0;
    int a = -1;
    float[] xArray = new float[arrayLength];
    float[] yArray = new float[arrayLength];
    float[] zArray = new float[arrayLength];
    int zeroCount = 0;

    //public XRGrabInteractable GrabInteractable;
    public InputActionReference buttonPress = null;
    private Transform transform = null;

    bool isActive = false;
    private string letterInput;

    public Transform trans;
    public GameObject gameObj;

    private void Start()
    {
        File.Create(filepath); //data collection mode
    }
    private void Awake() //when trigger is pressed
    {
        buttonPress.action.performed += Toggle;
    }

    private void OnDestroy() //when trigger is pressed
    {
        buttonPress.action.started -= Toggle;
    }

    private void Toggle(InputAction.CallbackContext context)
    {
        isActive = !isActive;
        StartCoroutine(PrintCoordinates());
        Debug.Log("Recording Start/Stop");
        //gameObject.SetActive(isActive);
    }

    private IEnumerator PrintCoordinates()
    {
        using (StreamWriter writer = new StreamWriter(filepath)) //to write into folder
        {
            /*      - remove comments to write the database header
            writer.Write("X1,Y1,Z1");
            for (int Outer = 2; Outer <= arrayLength; Outer++) //prints the top of row of the data for machine learning
            {
                for (char Inner = 'X'; Inner <= 'Z'; Inner++)
                {
                    writer.Write("," + Inner.ToString());
                    writer.Write(Outer);
                }
            }
            writer.Write("\n");
            // */

            //Debug.Log("Recording coordinates");
            while (isActive == true)
            {

                //obtains the first coordinate point (example: (10, 20, 30))
                float xWorldPosition = gameObject.transform.position.x;
                float yWorldPosition = gameObject.transform.position.y;
                float zWorldPosition = gameObject.transform.position.z;
                

                for (int i = 0; i <= arrayLength - 1; i++)
                {
                    //subtracts the first coordinate point by itself and sets it to a converison ( (10, 20, 30) -> (0,0,0) )
                    float xLocalPosition = gameObject.transform.position.x - xWorldPosition;
                    float yLocalPosition = gameObject.transform.position.y - yWorldPosition;
                    float zLocalPosition = gameObject.transform.position.z - zWorldPosition;

                    a++;
                    //Debug.Log("a: " + a);

                    if (isActive == true) //records local points while record button is being pressed
                    {
                        xArray[a] = xLocalPosition;
                        yArray[a] = yLocalPosition;
                        zArray[a] = zLocalPosition;
                        yield return new WaitForSeconds(0.025f); //delay - 0.05f
                    }
                    else //when the record button is released, the rest of the array will be zeros
                    {
                        xArray[a] = 0;
                        yArray[a] = 0;
                        zArray[a] = 0;
                        zeroCount++;

                        yield return new WaitForSeconds(0.001f); //delay
                        //Debug.Log("Zero count: " + zeroCount);
                    }
                }

                yield return new WaitForSeconds(0.001f); //delay
            }
           
            //lists created
            List<float> xList = new List<float>();
            List<float> yList = new List<float>();
            List<float> zList = new List<float>();

            //Convert arrays to lists
            xList = arrayToList(xArray);
            yList = arrayToList(yArray);
            zList = arrayToList(zArray);

            //the following is used for data collection

            //loop that destributes the data
            xList = distributeLoop(xList, zeroCount, arrayLength);
            yList = distributeLoop(yList, zeroCount, arrayLength);
            zList = distributeLoop(zList, zeroCount, arrayLength);

            //writes into .data file
            writer.WriteLine(xList[0]); //writes to file
            writer.WriteLine(yList[0]);
            writer.WriteLine(zList[0]);
            for (int i = 1; i < xList.Count; i++)
            {
                writer.WriteLine(xList[i]);
                writer.WriteLine(yList[i]);
                writer.WriteLine(zList[i]);
            }
        }
        i = 0;
        a = -1;
        zeroCount = 0;
    }

    private List<float> distributeLoop(List<float> list1, int zeroCount, int arrayLength)
    {
        List<float> list2 = new List<float>();
        int b = 0; //Loop counter
        int normalDigitCount = arrayLength - zeroCount;

        while (zeroCount != 0) //zeroCount does not include the array[0]
        {
            var tuple = distribute(list1, normalDigitCount, zeroCount); //touple.Item1 = array, touple.Item2 = normalDigitCount, touple.Item3 = zeroCount
            list1 = tuple.Item1; //sets the post-distribute array to yArray
            normalDigitCount = tuple.Item2;
            zeroCount = tuple.Item3;
            b++;
            if (zeroCount == 0)
            {
                Debug.Log("Distribution Complete"); //displays  zeroCount
            }
        }

        return list1;
    }

    private List<float> arrayToList(float[] array)
    {
        List<float> list1 = new List<float>();

        foreach (var a in array)
        {
            list1.Add(a);
        }

        return list1;
    }

    private Tuple<List<float>, int, int> distribute(List<float> list1, int normalDigitCount, int zeroCount)
    {
        int tempZeroCount = zeroCount;
        List<float> list2 = new List<float>();
        List<float> list3 = new List<float>();

        for (int i = 0; i < normalDigitCount; i++)
        {
            if (tempZeroCount != 0)
            {
                list2.Add(list1[i]);
                list2.Add(0); //creates an unnessary 0 at the end but is removed later
                tempZeroCount--;
            }
            else
            {
                list2.Add(list1[i]);
            }
        }

        if (list2[list2.Count - 1] == 0)
        {
            tempZeroCount++;
            list2.RemoveAt(list2.Count - 1);
        }

        list3 = zeroAssign(list2);

        for (int j = 0; j < tempZeroCount; j++)
        {
            list3.Add(0);
        }

        normalDigitCount = list3.Count - tempZeroCount;

        return new Tuple<List<float>, int, int>(list3, normalDigitCount, tempZeroCount); //makes the function able to return multiple values for proper looping
    }

    private List<float> zeroAssign(List<float> list1)
    {
        List<float> list2 = new List<float>();

        int listLength = list1.Count;

        i = 1; //element 0 = 0, so by setting it to 1 it goes to element 1
        list2.Add(0);
        while (i < listLength)
        {
            if (list1[i] != 0)
            {
                list2.Add(list1[i]);
            }
            else
            {
                i--;
                list2.Add(list1[i]);
                i++;
            }
            i++;
        }

        return list2;
    }
}