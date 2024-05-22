﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using YARG.Core.Game;
using YARG.Core.Logging;
using YARG.Gameplay.Player;
using YARG.Helpers;
using YARG.Helpers.Extensions;

namespace YARG.Gameplay.Visuals
{
    public class ProKeysTrackOverlay : MonoBehaviour
    {
        [SerializeField]
        private GameObject _keyOverlayPrefab;

        [SerializeField]
        private Texture2D _edgeGradientTexture;

        [SerializeField]
        private Texture2D _heldGradientTexture;

        [SerializeField]
        private float _trackWidth = 2f;

        [Space]
        [SerializeField]
        private float _overlayOffset;

        public float KeySpacing => _trackWidth / ProKeysPlayer.WHITE_KEY_VISIBLE_COUNT;

        private List<GameObject> _highlights = new();

        private static readonly int IsHighlight = Shader.PropertyToID("_IsHighlight");
        private static readonly int BaseMap     = Shader.PropertyToID("_BaseMap");

        public void Initialize(TrackPlayer player, ColorProfile.ProKeysColors colors)
        {
            int overlayPositionIndex = 0;

            for (int i = 0; i < ProKeysPlayer.TOTAL_KEY_COUNT; i++)
            {
                int noteIndex = i % 12;
                int octaveIndex = i / 12;

                // Only create overlays on white keys
                if (PianoHelper.IsWhiteKey(noteIndex))
                {
                    var overlay = Instantiate(_keyOverlayPrefab, transform);
                    overlay.SetActive(true);
                    overlay.transform.localPosition = new Vector3(
                        overlayPositionIndex * KeySpacing + _overlayOffset, 0f, 0f);

                    // Get the group index (two groups per octave)
                    int index = octaveIndex * 2 + (PianoHelper.IsLowerHalfKey(noteIndex) ? 0 : 1);
                    var color = colors.GetOverlayColor(index).ToUnityColor();

                    var material = overlay.GetComponentsInChildren<MeshRenderer>()[0].material;
                    var highlightRenderer = overlay.GetComponentsInChildren<MeshRenderer>()[1];
                    var highlightMaterial = highlightRenderer.material;

                    color.a = 0.05f;
                    material.color = color;
                    material.SetFade(player.ZeroFadePosition, player.FadeSize);

                    switch (i)
                    {
                        case 0:
                        case 4:
                            material.SetTextureScale(BaseMap, new Vector2(i == 4 ? -1f : 1f, 1f));
                            material.SetTexture(BaseMap, _edgeGradientTexture);
                            break;
                        case 5:
                        case 11:
                            material.SetTextureScale(BaseMap, new Vector2(i == 11 ? -1f : 1f, 1f));
                            material.SetTexture(BaseMap, _edgeGradientTexture);
                            break;
                        case 12:
                        case 16:
                            material.SetTextureScale(BaseMap, new Vector2(i == 16 ? -1f : 1f, 1f));
                            material.SetTexture(BaseMap, _edgeGradientTexture);
                            break;
                        case 17:
                        case 23:
                            material.SetTextureScale(BaseMap, new Vector2(i == 23 ? -1f : 1f, 1f));
                            material.SetTexture(BaseMap, _edgeGradientTexture);
                            break;
                    }

                    color.a = 0.3f;
                    highlightMaterial.color = color;
                    highlightMaterial.SetTexture(BaseMap, _heldGradientTexture);
                    highlightMaterial.SetFade(player.ZeroFadePosition, player.FadeSize);

                    highlightRenderer.gameObject.SetActive(false);
                    _highlights.Add(highlightRenderer.gameObject);

                    overlayPositionIndex++;
                }
            }
        }

        public void SetKeyHeld(int keyIndex, bool held)
        {
            if (keyIndex < 0 || keyIndex >= _highlights.Count) return;

            YargLogger.LogFormatDebug("Setting key held: {0}", keyIndex);

            _highlights[keyIndex].SetActive(held);
        }
    }
}