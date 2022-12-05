using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsInput;
using WindowsInput.Native;

[System.Serializable]
public class Cameralist_region {
    public List<Camera> regioncameras;
}

public class ProgramController : MonoBehaviour {
    public List<GameObject> MenuPanels;
    public List<GameObject> AutoList;
    public List<GameObject> ManualList;
    public List<Cameralist_region> CameraList;
    public Slider slider;
    public Text Timescale_;
    public Text Timelimit;

    public float Timescale;
    private bool menu;
    public int manual;
    private bool top;
    private bool[] keyOn = new bool[5];

    private void Start() {
        menu = false;
        top = false;
        manual = 0;
        for (int i = 0; i < 5; i++)
            keyOn[i] = false;
        Timescale = 1.5f;
        Time.timeScale = 0;
        MenuPanels[0].SetActive(true);
    }

    public void startProgram() {
        Time.timeScale = Timescale;
        MenuPanels[0].SetActive(false);
        changeView_TOP();
    }

    private void close_all_panel() {
        foreach (GameObject panels in MenuPanels)
            panels.SetActive(false);
    }
    public void Menu_button() {
        if (!menu) {
            Time.timeScale = 0; //게임 일시정지
            close_all_panel();
            MenuPanels[1].SetActive(true);
            menu = true;
        }
        else {
            Continue();
        }
    }

    public void Continue() {
        Time.timeScale = Timescale;
        close_all_panel();
        if (!top) {
           MenuPanels[6].SetActive(true);
        }
        else {
            if (manual != 0) MenuPanels[7].SetActive(true);
            else MenuPanels[5].SetActive(true);
        }
        menu = false;
    }

    public void openPanel(int index) {
        close_all_panel();
        MenuPanels[index].SetActive(true);
    }

    public void auto_mode() {
        foreach (GameObject parkingworld in AutoList)
            reset_parkingworld(parkingworld, true);
        foreach (GameObject parkingworld in ManualList)
            reset_parkingworld(parkingworld, false);
        Time.timeScale = Timescale;
        close_all_panel();
        MenuPanels[5].SetActive(true);
        manual = 0;
        menu = false;
    }

    public void manual_mode(int index) {
        foreach (GameObject parkingworld in AutoList)
            reset_parkingworld(parkingworld, false);
        for (int i = 0; i < 3; i++) {
            if (i == index) reset_parkingworld(ManualList[i], true);
            else reset_parkingworld(ManualList[i], false);
        }
        manual = index + 1;
        Time.timeScale = Timescale;
        changeView_TOP();
        MenuPanels[4].SetActive(true);
        MenuPanels[7].SetActive(true);
        menu = false;
    }

    public void reset_parkingworld(GameObject parkingworld, bool active) {
        SimulationManager manager = (SimulationManager)parkingworld.transform.Find("AgentPrefab").gameObject.GetComponent(typeof(SimulationManager));
        if (manager != null)
            manager.ResetSimulation();
        parkingworld.SetActive(active);
        if (active) {
            manager.InitializeSimulation();
            AutoParkAgent agent = (AutoParkAgent)parkingworld.transform.Find("AgentPrefab").gameObject.GetComponent(typeof(AutoParkAgent));
            if (agent)
                agent.StartCoroutine(agent.RevertMaterial());
        }
    }

    public void changeView_POV(int parkingworld) {
        foreach (Cameralist_region cameralist_region in CameraList)
            foreach (Camera camera in cameralist_region.regioncameras)
                camera.gameObject.SetActive(false);
        if (parkingworld == -1) {
            for (int i = 0; i < 3; i++) {
                if (ManualList[i].activeSelf) {
                    foreach (Camera camera in CameraList[i + 4].regioncameras)
                        camera.gameObject.SetActive(true);
                }
            }
        }
        else {
            foreach (Camera camera in CameraList[parkingworld].regioncameras)
                camera.gameObject.SetActive(true);
        }
        close_all_panel();
        MenuPanels[6].SetActive(true);
        top = false;
    }

    public void changeView_TOP() {
        CameraList[0].regioncameras[0].gameObject.SetActive(true);
        for (int i = 1; i < 7; i++) {
            foreach (Camera camera in CameraList[i].regioncameras)
                camera.gameObject.SetActive(false);
        }
        close_all_panel();
        if (manual != 0) MenuPanels[7].SetActive(true);
        else MenuPanels[5].SetActive(true);
        top = true;
    }

    private void Update() {
        if (keyOn[0]) {
            var sim = new InputSimulator();
            sim.Keyboard.KeyPress(VirtualKeyCode.VK_W);
        }
        if (keyOn[1]) {
            var sim = new InputSimulator();
            sim.Keyboard.KeyPress(VirtualKeyCode.VK_A);
        }
        if (keyOn[2]) {
            var sim = new InputSimulator();
            sim.Keyboard.KeyPress(VirtualKeyCode.VK_S);
        }
        if (keyOn[3]) {
            var sim = new InputSimulator();
            sim.Keyboard.KeyPress(VirtualKeyCode.VK_D);
        }
        if (keyOn[4]) {
            var sim = new InputSimulator();
            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
        }
        timeSlider();
        if (manual != 0) {
            check_timelimit(manual);
        }
    }

    public void timeSlider() {
        Timescale = slider.value;
        Timescale_.text = Math.Round(slider.value, 1).ToString();
    }

    public void check_timelimit(int manual) {
        AutoParkAgent agent = (AutoParkAgent)ManualList[manual - 1].transform.Find("AgentPrefab").gameObject.GetComponent(typeof(AutoParkAgent));
        Timelimit.text = Math.Round(30 - agent.timer).ToString();
    }

    public void monitor_touch(int key, int mode) {
        if (mode == 1)
            keyOn[key] = true;
        else
            keyOn[key] = false;
    }

    public void GameExit() {
        Application.Quit();
    }
}