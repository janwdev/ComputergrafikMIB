using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
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
    [FuseeApplication(Name = "Tut09_HierarchyAndInput", Description = "Yet another FUSEE App.")]
    public class Tut09_HierarchyAndInput : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private float _camAngle = 0;
        private Transform _baseTransform;
        private Transform _bodyTransform;
        private Transform _upperArmTransform;
        private Transform _foreArmTransform;
        private Transform _baseHandTransform;
        private Transform _leftHandTransform;
        private Transform _rightHandTransform;

        private Boolean shouldOpenHand = false;
        private Boolean shouldCloseHand = false;

        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };
            _bodyTransform = new Transform
            {
                Rotation = new float3(0, 1.2f, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 6, 0)
            };
            _upperArmTransform = new Transform
            {
                Rotation = new float3(0.8f, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2, 4, 0)
            };
            _foreArmTransform = new Transform
            {
                Rotation = new float3(0.8f, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 8, 0)
            };

            _baseHandTransform = new Transform
            {
                Rotation = new float3((float)Math.PI, 0, (float)Math.PI / 2),
                Scale = new float3(1, 1, 1),
                Translation = new float3(4, 10, 0)
            };
            _leftHandTransform = new Transform
            {
                Rotation = new float3((float)Math.PI / 2, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 8, 2)
            };
            _rightHandTransform = new Transform
            {
                Rotation = new float3((float)Math.PI / 2, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 0, 2)
            };

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    // GREY BASE
                    new SceneNode
                    {
                        Components = new List<SceneComponent>
                        {
                            // TRANSFROM COMPONENT
                            _baseTransform,

                            // SHADER EFFECT COMPONENT
                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.LightGrey, float4.Zero),

                            // MESH COMPONENT
                            SimpleMeshes.CreateCuboid(new float3(10, 2, 10))
                        }
                    },
                    // RED BODY
                    new SceneNode
                    {
                        Components = new List<SceneComponent>
                        {
                            _bodyTransform,
                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.Red, float4.Zero),
                            SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                        },
                        Children = new ChildList
                        {
                            // GREEN UPPER ARM
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    _upperArmTransform,
                                },
                                Children = new ChildList
                                {
                                    new SceneNode
                                    {
                                        Components = new List<SceneComponent>
                                        {
                                            new Transform
                                            {
                                                Rotation = new float3(0, 0, 0),
                                                Scale = new float3(1, 1, 1),
                                                Translation = new float3(0, 4, 0)
                                            },
                                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.Green, float4.Zero),
                                            SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                        }
                                    },
                                    // BLUE FOREARM
                                    new SceneNode
                                    {
                                        Components = new List<SceneComponent>
                                        {
                                            _foreArmTransform,
                                        },
                                        Children = new ChildList
                                        {
                                            new SceneNode
                                            {
                                                Components = new List<SceneComponent>
                                                {
                                                    new Transform
                                                    {
                                                        Rotation = new float3(0, 0, 0),
                                                        Scale = new float3(1, 1, 1),
                                                        Translation = new float3(0, 4, 0)
                                                    },
                                                    MakeEffect.FromDiffuseSpecular((float4) ColorUint.Blue, float4.Zero),
                                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                }
                                            },
                                            // HandBase
                                            new SceneNode
                                            {
                                                Components = new List<SceneComponent>
                                                {
                                                    _baseHandTransform,
                                                },
                                                Children = new ChildList
                                                {
                                                    new SceneNode
                                                    {
                                                        Components = new List<SceneComponent>
                                                        {
                                                            new Transform
                                                            {
                                                                Rotation = new float3(0, 0, 0),
                                                                Scale = new float3(1, 1, 1),
                                                                Translation = new float3(0, 4, 0)
                                                            },
                                                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.LightGrey, float4.Zero),
                                                            SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                        }
                                                    },
                                                    // HandLeft
                                                    new SceneNode
                                                    {
                                                        Components = new List<SceneComponent>
                                                        {
                                                            _leftHandTransform,
                                                        },
                                                        Children = new ChildList
                                                        {
                                                            new SceneNode
                                                            {
                                                                Components = new List<SceneComponent>
                                                                {
                                                                    new Transform
                                                                    {
                                                                        Rotation = new float3(0, 0, 0),
                                                                        Scale = new float3(1, 1, 1),
                                                                        Translation = new float3(0, 0, 0)
                                                                    },
                                                                    MakeEffect.FromDiffuseSpecular((float4) ColorUint.Blue, float4.Zero),
                                                                    SimpleMeshes.CreateCuboid(new float3(2, 6, 2))
                                                                }
                                                            }
                                                        }
                                                    },
                                                    // HandRight
                                                    new SceneNode
                                                    {
                                                        Components = new List<SceneComponent>
                                                        {
                                                            _rightHandTransform,
                                                        },
                                                        Children = new ChildList
                                                        {
                                                            new SceneNode
                                                            {
                                                                Components = new List<SceneComponent>
                                                                {
                                                                    new Transform
                                                                    {
                                                                        Rotation = new float3(0, 0, 0),
                                                                        Scale = new float3(1, 1, 1),
                                                                        Translation = new float3(0, 0, 0)
                                                                    },
                                                                    MakeEffect.FromDiffuseSpecular((float4) ColorUint.Blue, float4.Zero),
                                                                    SimpleMeshes.CreateCuboid(new float3(2, 6, 2))
                                                                }
                                                            },
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                        }
                    }
                }
            };
        }

        public void moveArm()
        {
            _bodyTransform.Rotation = _bodyTransform.Rotation + new float3(0, 1.5f * Keyboard.ADAxis * Time.DeltaTime, 0);
            _upperArmTransform.Rotation = _upperArmTransform.Rotation + new float3(1.5f * Keyboard.WSAxis * Time.DeltaTime, 0, 0);
            _foreArmTransform.Rotation = _foreArmTransform.Rotation + new float3(1.5f * Keyboard.UpDownAxis * Time.DeltaTime, 0, 0);
        }

        private void moveCam()
        {
            if (Mouse.LeftButton)
            {
                _camAngle = _camAngle - Mouse.Velocity.x / 1000;
            }
        }

        private void openCloseHand()
        {
            if (!shouldCloseHand && !shouldOpenHand)
            {
                if (Keyboard.GetKey(KeyCodes.O))
                {
                    shouldOpenHand = true;
                }
                else if (Keyboard.GetKey(KeyCodes.C))
                {
                    shouldCloseHand = true;
                }
            }
            if (shouldOpenHand)
            {
                openHand();
            }
            else if (shouldCloseHand)
            {
                closeHand();
            }
        }

        private void openHand()
        {
            // Open
            if (_leftHandTransform.Translation.y < 8 && _rightHandTransform.Translation.y > 0)
            {
                _leftHandTransform.Translation.y = _leftHandTransform.Translation.y + 1.5f * Time.DeltaTime;
                _rightHandTransform.Translation.y = _rightHandTransform.Translation.y - 1.5f * Time.DeltaTime;
            }
            else
            {
                shouldOpenHand = false;
            }
        }
        private void closeHand()
        {
            // Close
            if (_leftHandTransform.Translation.y > 5 && _rightHandTransform.Translation.y < 3)
            {
                _leftHandTransform.Translation.y = _leftHandTransform.Translation.y - 1.5f * Time.DeltaTime;
                _rightHandTransform.Translation.y = _rightHandTransform.Translation.y + 1.5f * Time.DeltaTime;
            }
            else
            {
                shouldCloseHand = false;
            }
        }


        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = CreateScene();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            SetProjectionAndViewport();

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Anim
            // Arm
            moveArm();
            // Cam
            moveCam();
            // Hand
            openCloseHand();

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
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