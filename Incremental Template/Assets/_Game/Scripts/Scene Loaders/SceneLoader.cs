﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        var persistData = PersistData.Instance;
        SceneManager.LoadScene(persistData.LastSceneIndex);
    }
}