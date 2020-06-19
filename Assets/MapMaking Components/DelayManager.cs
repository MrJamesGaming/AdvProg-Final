using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DelayManager
{
    public Queue<Target> targets;
    public Maps map;
    public List<string> lines;


    private static DelayManager currentDelayManager;


    public static DelayManager getInstanceDelayManager()
    {
        return currentDelayManager;
    }

    public DelayManager(Maps map)
    {
        currentDelayManager = this;
        lines = new List<string>();
        targets = new Queue<Target>();
        if(map == Maps.A_FOOL_MOON_NIGHT) {
            string path = @"C:\Users\fizzi\Blossoming Beats\Assets\CommonScripts\MapsMetaData\AFoolMoonNightMetaData.txt";
            lines = File.ReadAllLines(path).ToList();
            PopulateMap(lines);
        } else if(map == Maps.CHICKEN_MONKEY_DUCK) {
            string path = @"C:\Users\fizzi\Blossoming Beats\Assets\CommonScripts\MapsMetaData\ChickenMonkeyDuckMetaData.txt";
            lines = File.ReadAllLines(path).ToList();
            PopulateMap(lines);
        }else if(map == Maps.HOLD) {
            string path = @"C:\Users\fizzi\Blossoming Beats\Assets\CommonScripts\MapsMetaData\HoldMetaData.txt";
            lines = File.ReadAllLines(path).ToList();
            PopulateMap(lines);
        }
        
    }

    
    //constructor for a potential editor 
    public DelayManager(Queue<Target> targets)
    {
        this.targets = targets;
    } 
    
    private void PopulateMap(List<string> lines)
    {
        Target previousTarget = null;
        bool isFirst = true;
        foreach(string current in lines) {
            
            string[] words = current.Split(' ');
            
            float delay = float.Parse(words[0]);
            float x = float.Parse(words[1]);
            float y = float.Parse(words[2]);
            float z = float.Parse(words[3]);
            int comboNum = int.Parse(words[4]);
            if (!isFirst) {
                previousTarget.setNextComboNum(comboNum);
            }
            previousTarget = new Target(delay, new Vector3(x, y, z), comboNum);
            targets.Enqueue(previousTarget);
            Debug.Log(targets.Peek());
            isFirst = false;
        }
        targets.Peek().setNextComboNum(-1);
    }
}
