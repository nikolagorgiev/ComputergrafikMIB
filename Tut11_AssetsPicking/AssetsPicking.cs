using System;
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
    public class AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private TransformComponent _baseTransform;
        private TransformComponent _haus;
        private TransformComponent _unterarm;
        private TransformComponent _schaufel;
        private TransformComponent _reifenLV;
        private TransformComponent _reifenRV;
        private TransformComponent _reifenLH;
        private TransformComponent _reifenRH;
        private float velocity = 1;
        private float _camRotation;

        private ScenePicker _scenePicker;
        private PickResult _newPick;

        private PickResult _currentPick;
        private float3 _oldColor;
        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            // TRANSFROM COMPONENT
                            _baseTransform,

                            // SHADER EFFECT COMPONENT
                            new ShaderEffectComponent
                            {
                                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.7f, 0.7f, 0.7f), new float3(1, 1, 1), 5)
                            },

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
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = AssetStorage.Get<SceneContainer>("Bagger.fus");

            _scenePicker = new ScenePicker(_scene);

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);

            _haus = _scene.Children.FindNodes(node => node.Name == "Haus")?.FirstOrDefault()?.GetTransform();
            _unterarm = _scene.Children.FindNodes(node => node.Name == "Unterarm")?.FirstOrDefault()?.GetTransform();
            _schaufel = _scene.Children.FindNodes(node => node.Name == "Schaufel")?.FirstOrDefault()?.GetTransform();
            _reifenLV = _scene.Children.FindNodes(node => node.Name == "ReifenLV")?.FirstOrDefault()?.GetTransform();
            _reifenRV = _scene.Children.FindNodes(node => node.Name == "ReifenRV")?.FirstOrDefault()?.GetTransform();
            _reifenLH = _scene.Children.FindNodes(node => node.Name == "ReifenLH")?.FirstOrDefault()?.GetTransform();
            _reifenRH = _scene.Children.FindNodes(node => node.Name == "ReifenRH")?.FirstOrDefault()?.GetTransform();
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //_baseTransform.Rotation = new float3(0, M.MinAngle(TimeSinceStart), 0);

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if(Keyboard.ADAxis!=0)
                _camRotation += 0.05f * Keyboard.ADAxis;

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 40) * float4x4.CreateRotationX(-(float) Atan(15.0 / 40.0))* float4x4.CreateRotationY(_camRotation);

            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);
                _scenePicker.View = RC.View;
                _scenePicker.Projection = RC.Projection;

                List<PickResult> pickResults = _scenePicker.Pick(pickPosClip).ToList();
                
                if (pickResults.Count > 0)
                {
                    pickResults.Sort((a, b) => Sign(a.ClipPos.z - b.ClipPos.z));
                    _newPick = pickResults[0];
                }

                if (_newPick != _currentPick)
                {
                    if (_currentPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = _currentPick.Node.GetComponent<ShaderEffectComponent>();
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", _oldColor);
                    }
                    if (_newPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = _newPick.Node.GetComponent<ShaderEffectComponent>();
                        _oldColor = (float3)shaderEffectComponent.Effect.GetEffectParam("DiffuseColor");
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", new float3(1, 0.4f, 0.4f));
                    }
                    _currentPick = _newPick;
                }
            }

            if(_newPick?.Node.Name == "Haus")
            {
            float Haus = _haus.Rotation.y;
            Haus += velocity * Keyboard.LeftRightAxis * DeltaTime;
            _haus.Rotation = new float3(0, Haus, 0);
            }

            if(_newPick?.Node.Name == "Unterarm")
            {
            float unterarm = _unterarm.Rotation.z;
            unterarm += velocity * Keyboard.LeftRightAxis * DeltaTime;
            if(unterarm>-0.4f)
                {
                    unterarm=-0.4f;
                }
                else if(unterarm<-1.5f)
                {
                    unterarm=-1.5f;
            }
            _unterarm.Rotation = new float3(0,0, unterarm); 
            
            }

            if(_currentPick?.Node.Name == "Schaufel")
            {
            float schaufel = _schaufel.Rotation.z;
            schaufel += velocity * Keyboard.UpDownAxis * DeltaTime;
            if(schaufel>-0.4f)
                {
                    schaufel=-0.4f;
                }
                else if(schaufel<-1.5f)
                {
                    schaufel=-1.5f;
            }
            _schaufel.Rotation = new float3(0, 0, schaufel);
            }

            if(_currentPick?.Node.Name == "ReifenLV")
            {
            float reifenLV = _reifenLV.Rotation.z;
            reifenLV += velocity * Keyboard.WSAxis * DeltaTime;
            _reifenLV.Rotation = new float3(0, 0, reifenLV);
            }

            if(_currentPick?.Node.Name == "ReifenRV")
            {
            float reifenRV = _reifenRV.Rotation.z;
            reifenRV += velocity * Keyboard.WSAxis * DeltaTime;
            _reifenRV.Rotation = new float3(0, 0, reifenRV);
            }

            if(_currentPick?.Node.Name == "ReifenLH")
            {
            float reifenLH = _reifenLH.Rotation.z;
            reifenLH += velocity * Keyboard.WSAxis * DeltaTime;
            _reifenLH.Rotation = new float3(0, 0, reifenLH);
            }

            if(_currentPick?.Node.Name == "ReifenRH")
            {
            float reifenRH = _reifenRH.Rotation.z;
            reifenRH += velocity * Keyboard.WSAxis * DeltaTime;
            _reifenRH.Rotation = new float3(0, 0, reifenRH);
            }

            

            

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45� Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}
