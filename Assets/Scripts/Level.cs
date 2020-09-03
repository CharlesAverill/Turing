using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// LevelsRoot myDeserializedClass = JsonConvert.DeserializeObject<LevelsRoot>(myJsonResponse);
[Serializable]
public class Level    {
    public string levelName;
    public string description;
    public List<List<string>> tapes;
    public List<List<string>> solutions;

    public Level(string ln, string d, List<List<string>> t, List<List<string>> s){
      levelName = ln;
      description = d;
      tapes = t;
      solutions = s;
    }

    public Level copy(){
      return new Level(levelName, description, tapes, solutions);
    }
}

public class LevelsRoot    {
    public List<Level> levels;
}
