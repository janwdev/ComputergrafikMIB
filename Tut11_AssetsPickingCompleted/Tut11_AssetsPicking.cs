using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.Effects;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuseeApp
{
    [FuseeApplication(Name = "Tut11_AssetsPicking", Description = "Yet another FUSEE App.")]
    public class Tut11_AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private ScenePicker _scenePicker;
        private Transform _carTransform;
        private Transform _wheelBackR;
        private Transform _wheelBackL;
        private Transform _wheelFrontR;
        private Transform _wheelFrontL;
        private Transform _gabelHalterung;
        private Transform _gabelPlatte;
        private PickResult _currentPick;
        private float4 _oldColor;

        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _carTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode
                    {
                        Components = new List<SceneComponent>
                        {
                            // TRANSFROM COMPONENT
                            _carTransform,

                            // SHADER EFFECT COMPONENT
                            SimpleMeshes.MakeMaterial((float4) ColorUint.LightGrey),

                            // MESH COMPONENT
                            // SimpleAssetsPickinges.CreateCuboid(new float3(10, 10, 10))
                            SimpleMeshes.CreateCuboid(new float3(10, 10, 10))
                        }
                    },
                }
            };
        }


        // Init is called on startup. 
        public override void Init()
        {
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = AssetStorage.Get<SceneContainer>("Gabelstabler.fus");

            _carTransform = _scene.Children.FindNodes(node => node.Name == "Fahrzeug")?.FirstOrDefault()?.GetTransform();

            _wheelBackR = _scene.Children.FindNodes(node => node.Name == "Rad_RH")?.FirstOrDefault()?.GetTransform();
            _wheelBackL = _scene.Children.FindNodes(node => node.Name == "Rad_LH")?.FirstOrDefault()?.GetTransform();
            _wheelFrontL = _scene.Children.FindNodes(node => node.Name == "Rad_LV")?.FirstOrDefault()?.GetTransform();
            _wheelFrontR = _scene.Children.FindNodes(node => node.Name == "Rad_RV")?.FirstOrDefault()?.GetTransform();

            _gabelHalterung = _scene.Children.FindNodes(node => node.Name == "Gabel")?.FirstOrDefault()?.GetTransform();
            _gabelPlatte = _scene.Children.FindNodes(node => node.Name == "Gabel_Platte")?.FirstOrDefault()?.GetTransform();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
            _scenePicker = new ScenePicker(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            SetProjectionAndViewport();

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 40) * float4x4.CreateRotationX(-(float)Math.Atan(15.0 / 40.0));

            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);

                PickResult newPick = _scenePicker.Pick(RC, pickPosClip).OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        var ef = _currentPick.Node.GetComponent<DefaultSurfaceEffect>();
                        ef.SurfaceInput.Albedo = _oldColor;
                    }
                    if (newPick != null)
                    {
                        var ef = newPick.Node.GetComponent<SurfaceEffect>();
                        _oldColor = ef.SurfaceInput.Albedo;
                        ef.SurfaceInput.Albedo = (float4)ColorUint.OrangeRed;
                    }
                    _currentPick = newPick;
                }
            }

            if (_currentPick != null)
            {
                if (_currentPick.Node.Name == "Gabel_Platte")
                {
                    if (Keyboard.GetKey(KeyCodes.Up))
                    {
                        plattformUp();
                    }
                    else if (Keyboard.GetKey(KeyCodes.Down))
                    {
                        plattformDown();
                    }
                }
                if (_currentPick.Node.Name == "Gabel")
                {
                    if (Keyboard.GetKey(KeyCodes.Up))
                    {
                        rotHalterungUp();
                    }
                    else if (Keyboard.GetKey(KeyCodes.Down))
                    {
                        rotHalterungDown();
                    }
                }
                Diagnostics.Debug(_currentPick.Node.Name);
                if (_currentPick.Node.Name == "Rad_RH" || _currentPick.Node.Name == "Rad_LH" || _currentPick.Node.Name == "Rad_RV" || _currentPick.Node.Name == "Rad_LV")
                {
                    if (Keyboard.GetKey(KeyCodes.Left))
                    {
                        driveBackward();
                    }
                    else if (Keyboard.GetKey(KeyCodes.Right))
                    {
                        driveForward();
                    }
                }
                if (_currentPick.Node.Name == "Fahrzeug")
                {
                    if (Keyboard.GetKey(KeyCodes.Up))
                    {
                        _carTransform.Rotation = new float3(0, _carTransform.Rotation.y + 1.5f * Time.DeltaTime, 0);
                    }
                    else if (Keyboard.GetKey(KeyCodes.Down))
                    {
                        _carTransform.Rotation = new float3(0, _carTransform.Rotation.y - 1.5f * Time.DeltaTime, 0);
                    }

                    if (Keyboard.GetKey(KeyCodes.Left))
                    {
                        driveBackward();
                    }
                    else if (Keyboard.GetKey(KeyCodes.Right))
                    {
                        driveForward();
                    }
                }
            }

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
            Present();
        }

        public void plattformUp()
        {
            if (_gabelPlatte.Translation.y < 2)
            {
                _gabelPlatte.Translation.y = _gabelPlatte.Translation.y + 1.5f * Time.DeltaTime;
            }
        }
        public void plattformDown()
        {
            if (_gabelPlatte.Translation.y > -2)
            {
                _gabelPlatte.Translation.y = _gabelPlatte.Translation.y - 1.5f * Time.DeltaTime;
            }
        }

        public void rotHalterungUp()
        {
            if (_gabelHalterung.Rotation.z < Math.PI / 6)
            {
                _gabelHalterung.Rotation.z = _gabelHalterung.Rotation.z + 1.5f * Time.DeltaTime;
            }
        }

        public void rotHalterungDown()
        {
            if (_gabelHalterung.Rotation.z > -Math.PI / 6)
            {
                _gabelHalterung.Rotation.z = _gabelHalterung.Rotation.z - 1.5f * Time.DeltaTime;
            }
        }

        public void driveForward()
        {
            _wheelBackR.Rotation = new float3(0, M.MinAngle(TimeSinceStart), 0);
            _wheelBackL.Rotation = new float3((float)Math.PI, M.MinAngle(TimeSinceStart), 0);
            _wheelFrontL.Rotation = _wheelBackL.Rotation;
            _wheelFrontR.Rotation = _wheelBackR.Rotation;
            _carTransform.Translation.x = _carTransform.Translation.x + 1.5f * Time.DeltaTime;
        }

        public void driveBackward()
        {
            _wheelBackR.Rotation = new float3(0, -M.MinAngle(TimeSinceStart), 0);
            _wheelBackL.Rotation = new float3((float)Math.PI, -M.MinAngle(TimeSinceStart), 0);
            _wheelFrontL.Rotation = _wheelBackL.Rotation;
            _wheelFrontR.Rotation = _wheelBackR.Rotation;
            _carTransform.Translation.x = _carTransform.Translation.x - 1.5f * Time.DeltaTime;
        }

        public void SetProjectionAndViewport()
        {
            // Set the rendering area to the entire window size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}