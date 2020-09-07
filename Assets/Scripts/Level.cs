using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// LevelsRoot myDeserializedClass = JsonConvert.DeserializeObject<LevelsRoot>(myJsonResponse);
[Serializable]
public class Level    {
    public string levelName { get; set; }
    public string description { get; set; }
    public List<List<string>> tapes { get; set; }
    public List<List<string>> solutions { get; set; }

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
