﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EvoNet.AI;
using Microsoft.Xna.Framework.Input;

namespace EvoNet.Rendering
{
    class NeuralNetworkRenderer
    {

        NeuronalNetwork network;
        public EvoNet.AI.NeuronalNetwork Network
        {
            get { return network; }
            set { network = value; }
        }
        public const float NEURONSIZE = 15;

        private Neuron GetNeuronUnderMouse()
        {
            MouseState ms = Mouse.GetState();
            foreach (Neuron n in Network.InputNeurons)
            {
                if (n.IsMouseOverDrawPosition(NEURONSIZE, ms))
                {
                    return n;
                }
            }
            foreach (Neuron n in Network.HiddenNeurons)
            {
                if (n.IsMouseOverDrawPosition(NEURONSIZE, ms))
                {
                    return n;
                }
            }
            foreach (Neuron n in Network.OutputNeurons)
            {
                if (n.IsMouseOverDrawPosition(NEURONSIZE, ms))
                {
                    return n;
                }
            }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            if (network == null)
            {
                return;
            }
            CalculateNeuronsRenderPosition(rect);
            Neuron mouseNeuron = GetNeuronUnderMouse();
            float yMin = rect.Y + NEURONSIZE / 2;
            float yMax = rect.Y + rect.Height - NEURONSIZE / 2;
            float strongestConnection = GetStrongestConnection();
            DrawLayer(spriteBatch, network.OutputNeurons, strongestConnection, mouseNeuron, new Vector2(10, -10));
            DrawLayer(spriteBatch, network.HiddenNeurons, strongestConnection, mouseNeuron);
            DrawLayer(spriteBatch, network.InputNeurons, strongestConnection, mouseNeuron, new Vector2(-10, -10), true);
        }

        private void CalculateNeuronsRenderPosition(Rectangle rect)
        {
            float yMin = rect.Y + NEURONSIZE / 2;
            float yMax = rect.Y + rect.Height - NEURONSIZE / 2;
            CalculateNeuronsRederPositionLayer(network.OutputNeurons, rect.X + rect.Width - NEURONSIZE / 2, yMin, yMax);
            CalculateNeuronsRederPositionLayer(network.HiddenNeurons, rect.X + rect.Width / 2, yMin, yMax);
            CalculateNeuronsRederPositionLayer(network.InputNeurons, rect.X + NEURONSIZE / 2, yMin, yMax);
        }

        private void CalculateNeuronsRederPositionLayer(List<Neuron> layer, float x, float yMin, float yMax)
        {
            float yDiff = yMax - yMin;
            float distanceBetweenNeurons = yDiff / (layer.Count - 1);
            float currentY = yMin;
            for (int i = 0; i < layer.Count; i++)
            {
                layer[i].DrawPosition = new Vector2(x, currentY);
                currentY += distanceBetweenNeurons;
            }
        }

        private void DrawLayer(SpriteBatch spriteBatch, List<Neuron> layer, float strongestConnection, Neuron mouseNeuron, Vector2? nameOffset = null, bool writeRight = false)
        {
            for (int i = 0; i < layer.Count; i++)
            {
                DrawNeuron(spriteBatch, layer[i], strongestConnection, mouseNeuron, nameOffset, writeRight);
            }
        }

        private void DrawNeuron(SpriteBatch spriteBatch, Neuron n, float strongestConnection, Neuron mouseNeuron, Vector2? nameOffset = null, bool writeRight = false)
        {
            if(n is WorkingNeuron)
            {
                DrawConnections(spriteBatch, n, strongestConnection, mouseNeuron);
            }
            float x = n.DrawPosition.X;
            float y = n.DrawPosition.Y;
            Color c = Color.Black;
            float val = n.GetValue();
            if(val < 0)
            {
                c = Color.Red;
            }
            else
            {
                c = Color.Green;
            }

            float valSize = val * NEURONSIZE;

            RenderHelper.DrawCircle(spriteBatch, x, y, NEURONSIZE / 2 + 1, Color.White);
            RenderHelper.DrawCircle(spriteBatch, x, y, valSize / 2, c);

            if (nameOffset != null)
            {
                Vector2 pos = new Vector2(x, y) + (Vector2)nameOffset;
                if (writeRight)
                {
                    pos.X -= Fonts.FontArial.MeasureString(n.GetName()).X;
                }
                spriteBatch.DrawString(Fonts.FontArial, n.GetName(), pos, Color.White);
            }
        }

        private void DrawConnections(SpriteBatch spriteBatch, Neuron n, float strongestConnection, Neuron mouseNeuron)
        {
            WorkingNeuron wn = (WorkingNeuron)n;
            foreach(Connection c in wn.GetConnections())
            {
                if(mouseNeuron != null && n != mouseNeuron && c.entryNeuron != mouseNeuron)
                {
                    continue;
                }
                Color color = Color.Black;
                float value = c.GetValue();
                float alpha = Mathf.Sqrt(Math.Abs(value) / strongestConnection);
                //TODO
                if (value > 0)
                {
                    color = new Color(0f, alpha, 0f, 1f);
                }else
                {

                    color = new Color(alpha, 0f, 0f, 1f);
                }
                RenderHelper.DrawLine(spriteBatch, n.DrawPosition.X, n.DrawPosition.Y, c.entryNeuron.DrawPosition.X, c.entryNeuron.DrawPosition.Y, color, 1);
            }
        }

        public float GetStrongestConnection()
        {
            return Mathf.Max(GetStrongestLayerConnection(Network.HiddenNeurons), GetStrongestLayerConnection(Network.OutputNeurons));
        }

        private float GetStrongestLayerConnection(List<Neuron> layer)
        {
            float strongestConnection = 0;
            foreach (Neuron n in layer)
            {
                WorkingNeuron wn = (WorkingNeuron)n;
                float strongestNeuronConnection = Math.Abs(wn.GetStrongestConnection());
                if (strongestNeuronConnection > strongestConnection)
                {
                    strongestConnection = strongestNeuronConnection;
                }
            }
            return strongestConnection;
        }
        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
