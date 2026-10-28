using System;
using UnityEngine;

namespace DragonBallDefenders
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource sfxSource;
        private AudioSource beamSource;

        private AudioClip kiBlastClip;
        private AudioClip explosionClip;
        private AudioClip kamehamehaClip;
        private AudioClip superSaiyanClip;
        private AudioClip coinClip;
        private AudioClip senzuClip;
        private AudioClip alarmClip;
        private AudioClip victoryClip;
        private AudioClip gameOverClip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            beamSource = gameObject.AddComponent<AudioSource>();
            beamSource.playOnAwake = false;
            beamSource.loop = true;

            GenerateProceduralAudioClips();
        }

        private void GenerateProceduralAudioClips()
        {
            kiBlastClip = CreateToneClip("KiBlast", 0.12f, 880f, 330f, 0.4f, ToneType.SineChirp);
            explosionClip = CreateNoiseClip("Explosion", 0.4f, 0.5f);
            kamehamehaClip = CreateToneClip("Kamehameha", 1.5f, 220f, 660f, 0.4f, ToneType.SawBuzz);
            superSaiyanClip = CreateToneClip("SuperSaiyan", 0.8f, 150f, 900f, 0.5f, ToneType.PowerUpSweep);
            coinClip = CreateToneClip("Coin", 0.15f, 987f, 1318f, 0.35f, ToneType.Arpeggio);
            senzuClip = CreateToneClip("Senzu", 0.3f, 440f, 880f, 0.4f, ToneType.HealChime);
            alarmClip = CreateToneClip("Alarm", 0.2f, 1000f, 600f, 0.5f, ToneType.SquareAlarm);
            victoryClip = CreateToneClip("Victory", 0.8f, 523f, 1046f, 0.45f, ToneType.VictoryFanfare);
            gameOverClip = CreateToneClip("GameOver", 0.7f, 400f, 120f, 0.5f, ToneType.SineChirp);
        }

        public void PlayKiBlastSound(float pitchJitter = 0.1f)
        {
            if (kiBlastClip != null)
            {
                sfxSource.pitch = UnityEngine.Random.Range(1f - pitchJitter, 1f + pitchJitter);
                sfxSource.PlayOneShot(kiBlastClip, 0.5f);
            }
        }

        public void PlayExplosionSound(float volume = 0.6f)
        {
            if (explosionClip != null)
            {
                sfxSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
                sfxSource.PlayOneShot(explosionClip, volume);
            }
        }

        public void PlayKamehamehaSound()
        {
            if (kamehamehaClip != null)
            {
                sfxSource.pitch = 1f;
                sfxSource.PlayOneShot(kamehamehaClip, 0.7f);
            }
        }

        public void PlaySuperSaiyanSound()
        {
            if (superSaiyanClip != null)
            {
                sfxSource.pitch = 1f;
                sfxSource.PlayOneShot(superSaiyanClip, 0.8f);
            }
        }

        public void PlayCoinSound()
        {
            if (coinClip != null)
            {
                sfxSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                sfxSource.PlayOneShot(coinClip, 0.5f);
            }
        }

        public void PlaySenzuHealSound()
        {
            if (senzuClip != null)
            {
                sfxSource.pitch = 1f;
                sfxSource.PlayOneShot(senzuClip, 0.6f);
            }
        }

        public void PlayAlarmSound()
        {
            if (alarmClip != null)
            {
                sfxSource.pitch = 1f;
                sfxSource.PlayOneShot(alarmClip, 0.6f);
            }
        }

        public void PlayVictorySound()
        {
            if (victoryClip != null)
            {
                sfxSource.pitch = 1f;
                sfxSource.PlayOneShot(victoryClip, 0.8f);
            }
        }

        public void PlayGameOverSound()
        {
            if (gameOverClip != null)
            {
                sfxSource.pitch = 1f;
                sfxSource.PlayOneShot(gameOverClip, 0.8f);
            }
        }

        private enum ToneType { SineChirp, SawBuzz, PowerUpSweep, Arpeggio, HealChime, SquareAlarm, VictoryFanfare }

        private AudioClip CreateToneClip(string name, float length, float startFreq, float endFreq, float volume, ToneType type)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * length);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, t);
                float envelope = (1f - t); // fade out
                float timeSec = (float)i / sampleRate;

                float sample = 0f;
                switch (type)
                {
                    case ToneType.SineChirp:
                        sample = Mathf.Sin(2f * Mathf.PI * currentFreq * timeSec) * envelope;
                        break;
                    case ToneType.SawBuzz:
                        float phase = (currentFreq * timeSec) % 1f;
                        sample = (2f * phase - 1f) * envelope * (0.8f + 0.2f * Mathf.Sin(20f * Mathf.PI * timeSec));
                        break;
                    case ToneType.PowerUpSweep:
                        float sweepFreq = Mathf.Lerp(startFreq, endFreq, Mathf.Pow(t, 2f));
                        sample = (Mathf.Sin(2f * Mathf.PI * sweepFreq * timeSec) + 0.5f * Mathf.Sin(4f * Mathf.PI * sweepFreq * timeSec)) * envelope;
                        break;
                    case ToneType.Arpeggio:
                        int noteIndex = (int)(t * 4);
                        float arpF = startFreq * Mathf.Pow(1.25f, noteIndex);
                        sample = Mathf.Sin(2f * Mathf.PI * arpF * timeSec) * envelope;
                        break;
                    case ToneType.HealChime:
                        sample = (Mathf.Sin(2f * Mathf.PI * currentFreq * timeSec) + 0.5f * Mathf.Sin(2f * Mathf.PI * (currentFreq * 1.5f) * timeSec)) * Mathf.Sin(Mathf.PI * t);
                        break;
                    case ToneType.SquareAlarm:
                        sample = (Mathf.Sin(2f * Mathf.PI * currentFreq * timeSec) > 0 ? 1f : -1f) * envelope * 0.4f;
                        break;
                    case ToneType.VictoryFanfare:
                        int step = Mathf.FloorToInt(t * 5);
                        float stepFreq = startFreq * (step == 0 ? 1f : (step == 1 ? 1.25f : (step == 2 ? 1.5f : (step == 3 ? 1.75f : 2f))));
                        sample = Mathf.Sin(2f * Mathf.PI * stepFreq * timeSec) * (1f - (t % 0.2f) * 4f);
                        break;
                }

                samples[i] = sample * volume;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateNoiseClip(string name, float length, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * length);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float envelope = Mathf.Exp(-t * 6f);
                float noise = (UnityEngine.Random.value * 2f - 1f);
                samples[i] = noise * envelope * volume;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
