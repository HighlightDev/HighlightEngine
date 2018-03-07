using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FixedPipelineLight
{
    public class OGLLigt
    {
        private float[] _diffuseLight;
        private float[] _ambientLight;
        private float[] _specularLight;
        private float[] _emissionLight;
        private float[] _specRef;
        private float _shininess;
        private float[] _positionPoint { set; get; }
        private float[] _directionPoint { set; get; }
        private LightName _currentLight { set; get; }
        private double _arcAngle { set; get; }
        private double _arcRadius { set; get; }
        private float[] _attenuationValues { set; get; }

        public void enableThisLight()
        {
            GL.Enable((EnableCap)_currentLight);
        }
        public void enableColorMaterial(MaterialFace side)
        {
            if (_ambientLight.Any(value => value != 0.0f ? true : false))  //Если значение было изменено - включаем отражение
            {
                GL.Material(side,MaterialParameter.Ambient, _ambientLight);
            }
            if (_diffuseLight.Any(value => value != 0.0f ? true : false))  //Если значение было изменено - включаем отражение
            {
                GL.Material(side, MaterialParameter.Diffuse, _diffuseLight);
            }
            if (_specularLight.Any(value => value != 0.0f ? true : false))  //Если значение было изменено - включаем отражение
            {
                GL.Material(side, MaterialParameter.Specular, _specRef);
                GL.Material(side, MaterialParameter.Shininess, _shininess);
            }
            if (_emissionLight.Any(value => value != 0.0f ? true : false))  //Если значение было изменено - включаем отражение
            {
                GL.Material(side, MaterialParameter.Emission, _emissionLight);
            }
        }
        public void enableLightAttributes()
        {
            GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, _attenuationValues[0]);
            GL.Light(LightName.Light0, LightParameter.LinearAttenuation, _attenuationValues[1]);
            GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, _attenuationValues[2]);

            GL.Light(_currentLight,LightParameter.Ambient, _ambientLight);        //Set Ambient light
            GL.Light(_currentLight, LightParameter.Diffuse, _diffuseLight);        //Set Diffuse light
            if (_specularLight.Any(value => value != 0.0f ? true : false))  //Если значение было изменено - включаем отражение
            {
                GL.Light(_currentLight,LightParameter.Specular, _specularLight);
            }
            GL.Light(_currentLight,LightParameter.Position, _positionPoint);           //Set light position
        }

        public void disableThisLight()
        {
            GL.Disable((EnableCap)_currentLight);
        }

        public void setCurrentLight(int number)
        {
            switch (number)
            {
                case 0:
                    {
                        this._currentLight = LightName.Light0;
                        break;
                    }
                case 1:
                    {
                        this._currentLight = LightName.Light1;
                        break;
                    }
                case 2:
                    {
                        this._currentLight = LightName.Light2;
                        break;
                    }
                case 3:
                    {
                        this._currentLight = LightName.Light3;
                        break;
                    }
                case 4:
                    {
                        this._currentLight = LightName.Light4;
                        break;
                    }
                case 5:
                    {
                        this._currentLight = LightName.Light5;
                        break;
                    }
                case 6:
                    {
                        this._currentLight = LightName.Light6;
                        break;
                    }
                case 7:
                    {
                        this._currentLight = LightName.Light7;
                        break;
                    }
                default:
                    {
                        this._currentLight = LightName.Light0;
                        break;
                    }
            }
        }
        public void setSpotlight(float spotCutoff, float spotExponent)
        {
            float[] vecDirection = new float[3] { _directionPoint[0] - _positionPoint[0], _directionPoint[1] - _positionPoint[1], _directionPoint[2] - _positionPoint[2] };
            GL.Light(_currentLight,LightParameter.SpotCutoff, spotCutoff);
            GL.Light(_currentLight,LightParameter.SpotDirection, vecDirection);
            if (spotExponent != 0.0f)
            {
                GL.Light(_currentLight,LightParameter.SpotExponent, spotExponent);
            }
        }
        public void setLightPosition(float x, float y, float z, float lightType)
        {
            _positionPoint[0] = x;
            _positionPoint[1] = y;
            _positionPoint[2] = z;
            _positionPoint[3] = lightType;
        }
        public void setLightPosition(Vector3 position, float lightType)
        {
            _positionPoint[0] = position.X;
            _positionPoint[1] = position.Y;
            _positionPoint[2] = position.Z;
            _positionPoint[3] = lightType;
        }
        public void setLightPositionFv(float[] position)
        {
            _positionPoint[0] = position[0];
            _positionPoint[1] = position[1];
            _positionPoint[2] = position[2];
            _positionPoint[3] = position[3];
        }
        public void setLightDirection(float x, float y, float z)
        {
            _directionPoint[0] = x;
            _directionPoint[1] = y;
            _directionPoint[2] = z;
        }
        public void setLightDirectionFv(float[] direction)
        {
            _directionPoint[0] = direction[0];
            _directionPoint[1] = direction[1];
            _directionPoint[2] = direction[2];
        }
        public void setLightDirection(Vector3 direction)
        {
            _directionPoint[0] = direction[0];
            _directionPoint[1] = direction[1];
            _directionPoint[2] = direction[2];
        }
        public void setAmbientLight(float red, float green, float blue, float alpha)
        {
            _ambientLight[0] = red;
            _ambientLight[1] = green;
            _ambientLight[2] = blue;
            _ambientLight[3] = alpha;
        }
        public void setDiffuseLight(float red, float green, float blue, float alpha)
        {
            _diffuseLight[0] = red;
            _diffuseLight[1] = green;
            _diffuseLight[2] = blue;
            _diffuseLight[3] = alpha;
        }
        public void setDiffuseLightFv(float[] diffuseLight)
        {
            for (int i = 0; i < 4; i++)
            {
                this._diffuseLight[i] = diffuseLight[i];
            }
        }
        public void setSpecularLight(float red, float green, float blue, float alpha)
        {
            _specularLight[0] = red;
            _specularLight[1] = green;
            _specularLight[2] = blue;
            _specularLight[3] = alpha;
        }
        public void setSpecularLightFv(float[] specularLight)
        {
            for (int i = 0; i < 4; i++)
            {
                this._specularLight[i] = specularLight[i];
            }
        }
        public void setEmissionLight(float red, float green, float blue, float alpha)
        {
            _emissionLight[0] = red;
            _emissionLight[1] = green;
            _emissionLight[2] = blue;
            _emissionLight[3] = alpha;
        }
        public void setSpecularRef(float red, float green, float blue, float alpha)
        {
            _specRef[0] = red;
            _specRef[1] = green;
            _specRef[2] = blue;
            _specRef[3] = alpha;
        }
        public void setSpecularRefFv(float[] specRef)
        {
            for (int i = 0; i < 4; i++)
            {
                this._specRef[i] = specRef[i];
            }
        }
        public void setSpecularShininess(float shiny)
        {
            _shininess = shiny;
        }
        public void setLightAttenuation(float constAttenuation, float linearAttenuation, float quadraticAttenuation)
        {
            _attenuationValues[0] = constAttenuation > 0.0f ? constAttenuation : 0.0f;
            _attenuationValues[1] = linearAttenuation > 0.0f ? linearAttenuation : 0.0f;
            _attenuationValues[2] = quadraticAttenuation > 0.0f ? quadraticAttenuation : 0.0f;
        }
        public OGLLigt()
        {
            _attenuationValues = new float[3];
            _attenuationValues[0] = 1.0f;
            _diffuseLight = new float[4];
            _ambientLight = new float[4];
            _specularLight = new float[4];
            _emissionLight = new float[4];
            _specRef = new float[4];
            _positionPoint = new float[4];
            _directionPoint = new float[3];
            _currentLight = LightName.Light0;
            _arcAngle = 0.0;
            _arcRadius = 10.0;
            _shininess = 10.0f;
        }
        /// <summary>
        /// Расширенный конструктор для направленного или ненаправленного света
        /// </summary>
        /// <param name="CurrentLight">Номер текущего источника света</param>
        /// <param name="positionV">Позиция света</param>
        /// <param name="ambientV">Рассеянный свет</param>
        /// <param name="diffuseV">Диффузный свет</param>
        /// <param name="specularV">Отражающий свет</param>
        /// <param name="specRefV">Отраженный свет</param>
        /// <param name="shininess">Сила солнечного блика( [0.0; 128.0] )</param>
        /// <param name="typeOfLight">1.0 если ненаправленный свет(точечный),0.0 если направленный</param>
        /// <param name="lightAttenuation">Световое убывание (постоянное,линейное,квадратичное)</param>
        public OGLLigt(int CurrentLight, Vector3 positionV, Vector3 ambientV, Vector3 diffuseV, Vector3 specularV,
            Vector3 specRefV, float shininess, float typeOfLight, Vector3 lightAttenuation)
            : this()
        {
            setCurrentLight(CurrentLight);
            setLightPosition(positionV[0], positionV[1], positionV[2], typeOfLight);
            setAmbientLight(ambientV[0], ambientV[1], ambientV[2], 1.0f);
            setDiffuseLight(diffuseV[0], diffuseV[1], diffuseV[2], 1.0f);
            setSpecularLight(specularV[0], specularV[1], specularV[2], 1.0f);
            setSpecularRef(specRefV[0], specRefV[1], specRefV[2], 1.0f);
            _shininess = shininess;
            _attenuationValues[0] = lightAttenuation[0];
            _attenuationValues[1] = lightAttenuation[1];
            _attenuationValues[2] = lightAttenuation[2];
        }
        /// <summary>
        /// Расширенный конструктор для прожектора
        /// </summary>
        /// <param name="CurrentLight">Номер текущего источника света</param>
        /// <param name="positionV">Позиция прожектора</param>
        /// <param name="direction">Направление прожектора</param>
        /// <param name="ambientV">Рассеянный свет</param>
        /// <param name="diffuseV">Диффузный свет</param>
        /// <param name="specularV">Отражающий свет</param>
        /// <param name="specRefV">Отраженный свет</param>
        /// <param name="shininess">Сила солнечного блика( [0.0; 128.0] )</param>
        public OGLLigt(int CurrentLight, Vector3 positionV, Vector3 direction, Vector3 ambientV, Vector3 diffuseV,
            Vector3 specularV, Vector3 specRefV, float shininess)
            : this()
        {
            setCurrentLight(CurrentLight);
            setLightPosition(positionV[0], positionV[1], positionV[2], 1.0f);
            setAmbientLight(ambientV[0], ambientV[1], ambientV[2], 1.0f);
            setDiffuseLight(diffuseV[0], diffuseV[1], diffuseV[2], 1.0f);
            setSpecularLight(specularV[0], specularV[1], specularV[2], 1.0f);
            setSpecularRef(specRefV[0], specRefV[1], specRefV[2], 1.0f);
            _shininess = shininess;
        }
    }
}
