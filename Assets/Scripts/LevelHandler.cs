using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class LevelHandler : MonoBehaviour
{

    public static LevelHandler lh;
    public GameObject tapePrefab;

    private List<Level> levels;
    public Level toLoad;
    public int levelIndex = -1;

    public int scenesLoaded = 0;

    public Tape currentTape;
    public int currentTapeIndex = -1;

    void OnEnable()
    {
      if(lh == null){
        DontDestroyOnLoad(gameObject);
        lh = this;

        toLoad = null;

        SceneManager.sceneLoaded += OnSceneLoad;

        TextAsset jsonAsset = Resources.Load<TextAsset>("levels");
        levels = JsonConvert.DeserializeObject<LevelsRoot>(jsonAsset.text).levels;
      }
      else if(lh != this){
        if(lh.scenesLoaded > scenesLoaded){
          Destroy(gameObject);
        }
        else{
          Destroy(lh.gameObject);
        }
      }
    }

    void OnDisable(){
      SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode){
      scenesLoaded += 1;
      if(toLoad == null && scene.name != "Title"){
        toLoad = levels[0];
      }
      if(toLoad != null){
        if(!loadNextTape()){
          Debug.Log("Level Load Error");
        }
      }
    }

    public bool loadTapeN(int n){
      if(currentTape != null){
        Destroy(currentTape.gameObject);
      }

      if(n >= toLoad.tapes.Count){
        return false;
      }

      GameObject tapeInstance = (GameObject)(Instantiate(tapePrefab));
      tapeInstance.transform.parent = GameObject.FindWithTag("TapeSpawnPoint").transform;

      currentTapeIndex = n;
      currentTape = tapeInstance.GetComponent<Tape>();
      currentTape.customTapeValues = toLoad.tapes[n].ToArray();
      currentTape.reinitialize();

      GameObject.FindWithTag("UIHandler").GetComponent<UIHandler>().tape = currentTape;

      return true;
    }

    public bool loadNextTape(){
      if(currentTape != null){
        Destroy(currentTape.gameObject);
      }

      if(currentTapeIndex + 1 >= toLoad.tapes.Count){
        return false;
      }

      GameObject tapeInstance = (GameObject)(Instantiate(tapePrefab));
      tapeInstance.transform.parent = GameObject.FindWithTag("TapeSpawnPoint").transform;

      currentTapeIndex += 1;
      currentTape = tapeInstance.GetComponent<Tape>();
      currentTape.customTapeValues = toLoad.tapes[currentTapeIndex].ToArray();
      currentTape.reinitialize();

      GameObject.FindWithTag("UIHandler").GetComponent<UIHandler>().tape = currentTape;

      return true;
    }

    public bool validateAnswer(){
      string[] submission = currentTape.getValues();
      string[] solution = toLoad.solutions[currentTapeIndex].ToArray();

      for(int i = 0; i < submission.Length; i++){
        if(submission[i] != solution[i]){
          return false;
        }
      }

      return true;
    }

    public void loadEditor(){
      currentTapeIndex = -1;
      SceneManager.LoadSceneAsync("Editor");
    }

    public void loadLevel(int n){
      toLoad = levels[n - 1].copy();
      levelIndex = n;
      loadEditor();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
