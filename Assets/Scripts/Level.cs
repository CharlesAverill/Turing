using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// LevelsRoot myDeserializedClass = JsonConvert.DeserializeObject<LevelsRoot>(myJsonResponse);
[Serializable]
public class Level    {
    public string levelName { get; private set; }
    public string description { get; private set; }
    public List<List<string>> tapes { get; private set; }
    public List<List<string>> solutions { get; private set; }

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
