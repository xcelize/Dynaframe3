using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3
{
    public class VideoPlayer
    {
        private bool _isPlaying;
        private Process? _videoProcess;
        private readonly Window _window;
        private readonly Panel _panel;

        public VideoPlayer(Window pMainWindow, ref Panel pPanel) 
        {
            _window = pMainWindow;
            _panel = pPanel;
            _videoProcess = null;
        }

        public void PlayVideo(string path)
        {
            _isPlaying = true;
            Logger.LogComment("Entering PlayVideoFile with Path: " + path);

            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.WindowStyle = ProcessWindowStyle.Maximized;

            // TODO: Parameterize omxplayer settings
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Logger.LogComment("Linux Detected, setting up OMX Player");
                pInfo.FileName = "omxplayer";
                Logger.LogComment("Setting up Appsettings...");
                pInfo.Arguments = AppSettings.Default.OXMOrientnation + " --aspect-mode " + AppSettings.Default.VideoStretch + " ";

                // Append volume command argument
                if (!AppSettings.Default.VideoVolume)
                {
                    pInfo.Arguments += "--vol -6000 ";
                }

                pInfo.Arguments += "\"" + path + "\"";
                Logger.LogComment("DF Playing: " + path);
                Logger.LogComment("OMXPLayer args: " + pInfo.Arguments);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pInfo.UseShellExecute = true;
                pInfo.FileName = "wmplayer.exe";
                pInfo.Arguments = "\"" + path + "\"";
                pInfo.Arguments += " /fullscreen";
                Logger.LogComment("Looking for media in: " + pInfo.Arguments);
            }

            _videoProcess = new Process();
            _videoProcess.StartInfo = pInfo;
            Logger.LogComment("PlayVideoFile: Starting player...");
            _videoProcess.Start();
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _window.Opacity = 0;
            });

            // Give video player time to start, then fade out to reveal it...
            System.Threading.Thread.Sleep(1000);
            Logger.LogComment("PlayVideoFile: Fading Foreground to reveal video player.");
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _window.Opacity = 0;
            });
        }

        public bool CheckStatus(bool ForceTransition)
        {
            if ((_videoProcess == null) || (_videoProcess.HasExited))
            {
                _isPlaying = false;
                KillVideoPlayer();

                Logger.LogComment("VideoPlayer CheckStatus returning false..: Video has exited!");
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _panel.Opacity = 1;
                    _window.Opacity = 1;
                });
                return false;
            }
            else if ((AppSettings.Default.PlaybackFullVideo == false) && (ForceTransition))
            {
                // We should interupt the video...check status gets called at the slide transition time.
                Logger.LogComment("VideoPlayer is closing playing video to transition to images..");
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _window.Opacity = 1;
                    _panel.Opacity = 1;
                });
                KillVideoPlayer();
                return true;
            }
            else
            {
                return true;
            }
        }

        public void KillVideoPlayer()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _panel.Opacity = 1;
                _window.Opacity = 1;
            });
            Logger.LogComment("KillVideoPlayer - Entering Method.");
            try
            {
                if (_videoProcess != null)
                {
                    try
                    {
                        _videoProcess.CloseMainWindow();
                        _videoProcess = null;
                    }
                    catch (InvalidOperationException)
                    {
                        // expected if the process isn't there.
                        Logger.LogComment("[INFO]: Le processus du player vidéo n'est pas rattaché");
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Tried and failed to kill video process..." + exc.ToString());
                        Logger.LogComment("Tried and failed to kill video process. Exception: " + exc.ToString());
                    }
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // OMXPlayer processes can be a bit tricky. to kill them we use
                    // killall - 9 omxplayer.bin
                    // -q quiets this down in case omxplayer isn't running

                    Helpers.RunProcess("killall", "-q -9 omxplayer.bin");
                    _videoProcess = null;

                }

            }
            catch (Exception)
            {
                // Swallow. This may no longer be there depending on what kills it (OMX player will exit if the video
                // completes for instance
            }
        }

        public bool IsPlaying { get { return _isPlaying; } }

    }
}
