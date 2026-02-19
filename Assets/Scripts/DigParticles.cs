using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(ParticleSystem))]
	[RequireComponent(typeof(ParticleSystemRenderer))]
	public class DigParticles : MonoBehaviour
	{
		private ParticleSystem particles;
		private ParticleSystemRenderer particleRenderer;

		public int emittedNumberPerTile = 10;

		private int particlesToEmit = 10;

		private void Awake()
		{
			particles = GetComponent<ParticleSystem>();
			particleRenderer = GetComponent<ParticleSystemRenderer>();
		}

		public void Configure(Color color, int width, int height)
		{
			var main = particles.main;
			main.startColor = color;

			var shape = particles.shape;
			shape.scale = new Vector3(width, height, 1.0f);

			particlesToEmit = width * height * emittedNumberPerTile;
		}

		public void Emit()
		{
			particles.Emit(particlesToEmit);
		}
	}
}
