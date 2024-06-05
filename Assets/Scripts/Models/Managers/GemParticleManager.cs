using System.Collections.Generic;
using UnityEngine;

namespace BlockShift
{
    public class GemParticleManager : MonoBehaviour 
    {
        public ParticleSystem YellowParticle;
        public ParticleSystem BlueParticle;
        public ParticleSystem GreenParticle;
        public ParticleSystem RedParticle;
        public ParticleSystem PurpleParticle;
        public ParticleSystem ComboHintParticle;
        public GameObject grid;
        
        public Dictionary<Block, PoolItem> usedParticleOnGrid; 

        private void Awake()
        {
            usedParticleOnGrid = new Dictionary<Block, PoolItem>();
        }

        public void PlayGemParticle(Block block)
        {
            PoolItemType particleSystemReference;

            switch (block.GetColor())
            {
                case BlockColor.Green:
                    particleSystemReference = PoolItemType.GreenParticle;
                    break;
                case BlockColor.Yellow:
                    particleSystemReference = PoolItemType.YellowParticle;
                    break;
                case BlockColor.Blue:
                    particleSystemReference = PoolItemType.BlueParticle;
                    break;
                case BlockColor.Red:
                    particleSystemReference = PoolItemType.RedParticle;
                    break;
                case  BlockColor.Purple:
                    particleSystemReference = PoolItemType.PurpleParticle;
                    break;
                default:
                    return;
            }

            var particle = PoolManager.instance.GetFromPool(particleSystemReference,grid.transform);
            particle.transform.localPosition = block.transform.localPosition;
            particle.transform.localScale = block.transform.localScale*2f;
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();

            particleSystem.Play(true);
        }

        public void PlayComboParticleOnItem(Block block)
        {
            var particle = PoolManager.instance.GetFromPool(PoolItemType.RainbowParticle, block.transform);
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localScale = new Vector3(0.79f,0.79f,1f);
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
            particleSystem.Play(true);
            usedParticleOnGrid.Add(block,particle);
        }

        public void StopComboParticle(Block block)
        {
            
            if (!usedParticleOnGrid.ContainsKey(block))
            {
                return;
            }
            else
            {
                PoolItem item = usedParticleOnGrid[block];
                usedParticleOnGrid.Remove(block);
                ParticleSystem particleSystem = item.GetComponent<ParticleSystem>();
                particleSystem.Stop();
                PoolManager.instance.ResetPoolItem(item);
            }
        }

        private void OnEnable()
        {
            GameManager.instance.SetParticleManager(this);
        }
    }
}
