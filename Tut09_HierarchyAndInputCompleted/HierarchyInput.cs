﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    
    public class HierarchyInput : RenderCanvas 
    {
        
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private float _openAngle = 0;
        private bool open = false;
        private TransformComponent _baseTransform;
        private TransformComponent _bodyTransform;
        private TransformComponent _upperArmTransform;
        private TransformComponent _foreArmTransform;
        private TransformComponent _greifHandBase;
        private TransformComponent _greifFingerLinks;
        private TransformComponent _greifFingerRechts;
        private float velocity = 1;

        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };
            _bodyTransform = new TransformComponent
            {
                Rotation = new float3(0, 1.2f, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 6, 0)
            };
            _upperArmTransform = new TransformComponent
            {
                Rotation = new float3(-0.8f, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2, 4, 0)
            };
            _foreArmTransform = new TransformComponent
            {
                Rotation = new float3(-0.8f, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 8, 0)
            };
            _greifHandBase = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 5.5f, -0.5f)
            };
            _greifFingerLinks = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2.5f, -0.5f, 1.0f)
            };
            _greifFingerRechts = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2.5f, -0.5f, 1.0f)
            };


            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    // GREY BASE
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            // TRANSFROM COMPONENT
                            _baseTransform,

                            // MATERIAL COMPONENT
                            new MaterialComponent
                            {
                                Diffuse = new MatChannelContainer { Color = new float3(0.7f, 0.7f, 0.7f) },
                                Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                            },
                            

                            // MESH COMPONENT
                            SimpleMeshes.CreateCuboid(new float3(10, 2, 10))
                        }
                    },
                    // RED BODY
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            _bodyTransform,
                            new MaterialComponent
                            {
                                Diffuse = new MatChannelContainer { Color = new float3(1, 0, 0) },
                                Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                            },
                            new ShaderEffectComponent
                            {
                                 Effect = SimpleMeshes.MakeShaderEffect(new float3(1,0,0),new float3(1,0,0), 5)
                            },
                            SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            // GREEN UPPER ARM
                            new SceneNodeContainer
                            {
                                Components = new List<SceneComponentContainer>
                                {
                                    _upperArmTransform,
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new TransformComponent
                                            {
                                                Rotation = new float3(0, 0, 0),
                                                Scale = new float3(1, 1, 1),
                                                Translation = new float3(0, 4, 0)
                                            },
                                            new MaterialComponent
                                            {
                                                Diffuse = new MatChannelContainer { Color = new float3(0, 1, 0) },
                                                Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                                            },
                                            new ShaderEffectComponent
                                            {
                                                Effect = SimpleMeshes.MakeShaderEffect(new float3(0,1,0),new float3(0,1,0), 5)
                                            },
                                            SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                        }
                                    },
                                    // BLUE FOREARM
                                    new SceneNodeContainer
                                    {
                                        Components = new List<SceneComponentContainer>
                                        {
                                            _foreArmTransform,
                                        },
                                        Children = new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new TransformComponent
                                                    {
                                                        Rotation = new float3(0, 0, 0),
                                                        Scale = new float3(1, 1, 1),
                                                        Translation = new float3(0, 4, 0)
                                                    },
                                                    new MaterialComponent
                                                    {
                                                        Diffuse = new MatChannelContainer { Color = new float3(0, 0, 1) },
                                                        Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                                                    },
                                                    new ShaderEffectComponent
                                                    {
                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0,0,1),new float3(0,0,1), 5)
                                                    },
                                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                },
                                                Children = new List<SceneNodeContainer>
                                                {
                                                    //GreifHand
                                                    new SceneNodeContainer
                                                    {
                                                        Components = new List<SceneComponentContainer>
                                                        {
                                                            _greifHandBase
                                                        },
                                                        Children = new List<SceneNodeContainer>
                                                        {
                                                            new SceneNodeContainer
                                                            {
                                                                Components = new List<SceneComponentContainer>
                                                                {
                                                                   new TransformComponent
                                                                    {
                                                                        Rotation = new float3(0, 0, 0),
                                                                        Scale = new float3(1, 1, 1),
                                                                        Translation = new float3(0, 0, 0)
                                                                    },
                                                                    new MaterialComponent
                                                                    {
                                                                        Diffuse = new MatChannelContainer { Color = new float3(0, 0, 1) },
                                                                        Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                                                                    },
                                                                    new ShaderEffectComponent
                                                                    {
                                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0.5f,0.5f,0.5f),new float3(1,1,1), 5)
                                                                    },
                                                                    SimpleMeshes.CreateCuboid(new float3(6, 1, 1)) 
                                                                },
                                                                Children = new List<SceneNodeContainer>
                                                                 {
                                                                    new SceneNodeContainer
                                                                    {
                                                                        Components = new List<SceneComponentContainer>
                                                                        {
                                                                            _greifFingerRechts
                                                                        },
                                                                        Children = new List<SceneNodeContainer>
                                                                        {
                                                                               new SceneNodeContainer
                                                                            {
                                                                                Components = new List<SceneComponentContainer>
                                                                                {
                                                                                    new TransformComponent
                                                                                    {
                                                                                        Rotation = new float3(0, 0, 0),
                                                                                        Scale = new float3(1, 1, 1),
                                                                                        Translation = new float3(0, 2, 0)
                                                                                    },
                                                                                    new MaterialComponent
                                                                                    {
                                                                                        Diffuse = new MatChannelContainer { Color = new float3(0, 0, 1) },
                                                                                        Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                                                                                    },
                                                                                    new ShaderEffectComponent
                                                                                    {
                                                                                    Effect = SimpleMeshes.MakeShaderEffect(new float3(0,0,0),new float3(1,1,1), 5)
                                                                                    },
                                                                                    SimpleMeshes.CreateCuboid(new float3(1,4.3f,1))
                                                                                 }
                                                                            }
                                                                        }
                                                                                    
                                                                    },
                                                                    new SceneNodeContainer
                                                                    {
                                                                        Components = new List<SceneComponentContainer>
                                                                        {
                                                                            _greifFingerLinks
                                                                        },
                                                                        Children = new List<SceneNodeContainer>
                                                                        {
                                                                            new SceneNodeContainer
                                                                            {
                                                                                Components = new List<SceneComponentContainer>
                                                                                {
                                                                                    new TransformComponent
                                                                                    {
                                                                                        Rotation = new float3(0, 0, 0),
                                                                                        Scale = new float3(1, 1, 1),
                                                                                        Translation = new float3(0, 2, 0)
                                                                                    },
                                                                                    new MaterialComponent
                                                                                    {
                                                                                        Diffuse = new MatChannelContainer { Color = new float3(0, 0, 1) },
                                                                                        Specular = new SpecularChannelContainer { Color = new float3(1, 1, 1), Shininess = 5 }
                                                                                    },
                                                                                    new ShaderEffectComponent
                                                                                    {
                                                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0,0,0),new float3(1,1,1), 5)
                                                                                    },
                                                                                    SimpleMeshes.CreateCuboid(new float3(1,4.3f,1))
                                                                                }
                                                                            }
                                                                        }                                                                                            
                                                                                        
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                 }
                                             }
                                         }
                                     }
                                                
                                }
                            }
                        }
                     }
                 } 
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = CreateScene();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            float bodyRot = _bodyTransform.Rotation.y;
            bodyRot += velocity * Keyboard.LeftRightAxis * DeltaTime;
            _bodyTransform.Rotation = new float3(0, bodyRot, 0);

            float upperGreen = _upperArmTransform.Rotation.x;
            upperGreen += velocity * Keyboard.UpDownAxis * DeltaTime;
            _upperArmTransform.Rotation = new float3(upperGreen, 0, 0);

            float forBlue = _foreArmTransform.Rotation.x;
            forBlue += velocity * Keyboard.WSAxis * DeltaTime;
            _foreArmTransform.Rotation = new float3(forBlue, 0, 0);

            /* float greifHand = _greifHandBase.Rotation.y;
            greifHand += velocity * Keyboard.Axis * DeltaTime;
            _greifHandBase.Rotation = new float3(0, greifHand, 0);*/

            float linksFinger = _greifFingerLinks.Rotation.z;
            linksFinger += velocity * Keyboard.ADAxis * DeltaTime;
            _greifFingerLinks.Rotation = new float3(0, 0, linksFinger);

            float rechtsFinger = _greifFingerRechts.Rotation.z;
            rechtsFinger += -velocity * Keyboard.ADAxis * DeltaTime;
            _greifFingerRechts.Rotation = new float3(0, 0, rechtsFinger);


            if (Mouse.LeftButton)
                {
                _camAngle += -Mouse.Velocity.x * DeltaTime * 0.008f;
                }

            if (rechtsFinger < -0.5f)
            {
                _greifFingerRechts.Rotation = new float3(0, 0, -0.5f);
                _greifFingerLinks.Rotation = new float3(0, 0, 0.5f);
            }
            else if (rechtsFinger > 0.5f)
            {
                _greifFingerRechts.Rotation = new float3(0, 0, 0.5f);
                _greifFingerLinks.Rotation = new float3(0, 0, -0.5f);
            }


            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
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