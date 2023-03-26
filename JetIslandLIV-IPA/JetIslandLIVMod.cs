using IllusionPlugin;
using LIV.SDK.Unity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JetIslandLIV {

    public class JetIslandLIVMod : IPlugin {

        private GameObject livObject;
        private Camera spawnedCamera;
        private static LIV.SDK.Unity.LIV livInstance;

        //IllusionPlugin interface - Mod name and version.
        public string Name {
            get { return "LIV Plugin"; }
        }
        public string Version {
            get { return "1.0"; }
        }

        public void Init() {
            TrySetupLiv();
        }

        public void DeInit() {
        }

        //IllusionPlugin interface method. - "Gets invoked when the application is started."
        public void OnApplicationStart() {
            SetUpLiv();
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            SystemLibrary.LoadLibrary("Plugins\\LIVAssets\\LIV_Bridge.dll");
        }

        //IllusionPlugin interface method. - "Gets invoked when the application is closed."
        public void OnApplicationQuit() {
        }


        //IllusionPlugin interface method. - "Gets invoked whenever a level is loaded."
        public void OnLevelWasLoaded(int level) {
            //_currentLevelId = level;
        }

        //IllusionPlugin interface method. - "Gets invoked after the first update cycle after a level was loaded."
        public void OnLevelWasInitialized(int level) {
            Init();
        }

        //IllusionPlugin interface method. - "Gets invoked on every physics update."
        public void OnFixedUpdate() {
        }

        //IllusionPlugin interface method. - "Gets invoked on every graphic update."
        public void OnUpdate() {
            if (Input.GetKeyDown(KeyCode.F3)) {
                Debug.Log(">>> F3: TrySetupLiv");
                TrySetupLiv();
            }

            UpdateFollowSpawnedCamera();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
            //This is only useful for when first loading into the game menu.
            TrySetupLiv();
        }

        public void TrySetupLiv() {
            /*
             * Jet Island
             * Unity 2018.4.2f1 Mono
             *
             * Camera names:
			 *   Camera
			 *   
			 * [CameraRig]menu
			 * [CameraRig]
			*/

            Camera[] arrCam = GameObject.FindObjectsOfType<Camera>().ToArray();
            //Debug.Log(">>> Camera count: " + arrCam.Length);
            foreach (Camera cam in arrCam) {
				//Debug.Log(cam.name);
                if (cam.name.Contains("LIV ")) {
                    continue;
                } else if (cam.name.Contains("Camera")) {
                    SetUpLiv(cam);
                    break;
                }
            }
        }

        private void UpdateFollowSpawnedCamera() {
            var livRender = GetLivRender();
            if (livRender == null || spawnedCamera == null) return;

            // When spawned objects get removed in Boneworks, they might not be destroyed and just be disabled.
            if (!spawnedCamera.gameObject.activeInHierarchy) {
                spawnedCamera = null;
                return;
            }

            var cameraTransform = spawnedCamera.transform;
            livRender.SetPose(cameraTransform.position, cameraTransform.rotation, spawnedCamera.fieldOfView);
        }

        private static void SetUpLiv() {
            AssetManager assetManager = new AssetManager("Plugins\\LIVAssets");
            var livAssetBundle = assetManager.LoadBundle("liv-shaders");
            SDKShaders.LoadFromAssetBundle(livAssetBundle);
        }

        private static Camera GetLivCamera() {
            try {
                return !livInstance ? null : livInstance.HMDCamera;
            } catch (Exception) {
                livInstance = null;
            }
            return null;
        }

        private static SDKRender GetLivRender() {
            try {
                return !livInstance ? null : livInstance.render;
            } catch (Exception) {
                livInstance = null;
            }
            return null;
        }

        private void SetUpLiv(Camera camera) {
            if (!camera) {
                Debug.Log("No camera provided, aborting LIV setup.");
                return;
            }

            var livCamera = GetLivCamera();
            if (livCamera == camera) {
                Debug.Log("LIV already set up with this camera, aborting LIV setup.");
                FixVisibility();
                return;
            }

            Debug.Log($"Setting up LIV with camera: {camera.name}...");
            if (livObject) {
                Object.Destroy(livObject);
            }

            var cameraParent = camera.transform.parent; //[CameraRig] or [CameraRig]menu

            /*
            GameObject cameraPrefab = GameObject.Find("LivCameraPrefab");
            if(cameraPrefab == null)
                cameraPrefab = new GameObject("LivCameraPrefab");

            cameraPrefab.SetActive(false);
            Camera cameraFromPrefab = cameraPrefab.AddComponent<Camera>();
            cameraFromPrefab.allowHDR = false;
            cameraFromPrefab.farClipPlane = 19000;
            cameraPrefab.transform.SetParent(cameraParent, false);
            */

            livObject = new GameObject("LIV");
            livObject.SetActive(false);

            livInstance = livObject.AddComponent<LIV.SDK.Unity.LIV>();
            livInstance.HMDCamera = camera;
            //livInstance.MRCameraPrefab = cameraFromPrefab;
            livInstance.stage = cameraParent;
            livInstance.fixPostEffectsAlpha = false;
            livInstance.spectatorLayerMask = ~0;
            livInstance.spectatorLayerMask &= ~(1 << (int)GameLayer.InvisibleInVR); //Body
            livInstance.spectatorLayerMask &= ~(1 << (int)GameLayer.NotInLiv); //Custom things we don't want like arms, hands
            
            livObject.SetActive(true);

            FixVisibility();
        }

        private void FixVisibility() {
            //Hide the arms from LIV output, they're named things like "Cube"
            GameObject ikp0 = GameObject.Find("ik parent");
            GameObject ikp1 = GameObject.Find("ik parent (1)");
            Transform[] ikp0c = ikp0.GetComponentsInChildren<Transform>();
            Transform[] ikp1c = ikp1.GetComponentsInChildren<Transform>();
            foreach (Transform part in ikp0c) {
                //Debug.Log(part.name);
                part.gameObject.layer = (int)GameLayer.NotInLiv;
            }
            foreach (Transform part in ikp1c) {
                //Debug.Log(part.name);
                part.gameObject.layer = (int)GameLayer.NotInLiv;
            }
            //Hide the hands from LIV output, done it this ridiculously way because of how they're named and I don't know how to use Unity functions better.
            GameObject controllerRight = GameObject.Find("Controller (right)");
            GameObject controllerLeft = GameObject.Find("Controller (left)");
            Transform hand0 = controllerRight.transform.Find("hand2"); //Right
            Transform hand1 = controllerLeft.transform.Find("hand2 (1)"); //Left
            if (hand0 != null && hand1 != null) {
                Transform hand0m = hand0.transform.Find("HandModel");
                Transform hand1m = hand1.transform.Find("HandModel");
                if (hand0m != null && hand1m != null) {
                    hand0m.gameObject.layer = hand1m.gameObject.layer = (int)GameLayer.NotInLiv;
                }
            }
            //Hide body - or just use InvisibleInVR
            //Helmet 1_Color 1/2/3
            //Torso 1_Color 1/2
        }
    }
}