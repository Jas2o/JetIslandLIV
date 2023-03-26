﻿using System;
using System.IO;
using UnityEngine;

namespace JetIslandLIV {

    public class AssetManager {
        private readonly string assetsDirectory;

        public AssetManager(string assetsDirectory) {
            this.assetsDirectory = assetsDirectory;
        }

        public AssetBundle LoadBundle(string assetName) {
            var bundlePath = Path.Combine(assetsDirectory, assetName);
            var bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null) {
                throw new Exception("Failed to load asset bundle " + bundlePath);
            }

            return bundle;
        }
    }
}