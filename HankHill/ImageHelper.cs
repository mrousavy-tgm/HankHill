﻿using ImageSharp;
using ImageSharp.Formats;
using System;
using System.IO;
using System.Net.Http;

namespace NeedsMoreJpeg {
    public static class ImageHelper {
        public static async void Jpegify(string url, Discord.WebSocket.ISocketMessageChannel channel) {
            string file = Path.GetTempFileName();
            string jpegified = Path.GetTempFileName().Replace(".tmp", ".jpg");

            try {
                using (HttpClient client = new HttpClient()) {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url)) {
                        using (
                            Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                            stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true)) {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Couldn't download Image! ({ex.Message})");
            }

            try {
                using (Image<Rgba32> image = Image.Load(file)) {
                    JpegEncoderOptions options = new JpegEncoderOptions {
                        Quality = 1,
                        Subsample = JpegSubsample.Ratio420
                    };
                    using (FileStream output = File.OpenWrite(jpegified)) {
                        image.SaveAsJpeg(output, options);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Couldn't jpegify Image! ({ex.Message})");
            }

            try {
                await channel.SendFileAsync(jpegified);
            } catch {
                try {
                    await channel.SendMessageAsync("I can't send Images here..");
                } catch {
                    // can't even send messages
                }
            }

            try {
                File.Delete(file);
                File.Delete(jpegified);
            } catch (Exception ex) {
                Console.WriteLine($"Couldn't cleanup! ({ex.Message})");
            }
        }

        public static async void Pixelate(string url, Discord.WebSocket.ISocketMessageChannel channel) {
            string file = Path.GetTempFileName();
            string pixelated = Path.GetTempFileName().Replace(".tmp", ".jpg");

            try {
                using (HttpClient client = new HttpClient()) {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url)) {
                        using (
                            Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                            stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true)) {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Couldn't download Image! ({ex.Message})");
            }

            try {
                using (Image<Rgba32> image = Image.Load(file)) {
                    JpegEncoderOptions options = new JpegEncoderOptions {
                        Quality = 60,
                        Subsample = JpegSubsample.Ratio420
                    };
                    using (FileStream output = File.OpenWrite(pixelated)) {
                        Image<Rgba32> pixelatedImage = image.Pixelate(8);
                        pixelatedImage.SaveAsJpeg(output, options);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Couldn't pixelate Image! ({ex.Message})");
            }

            try {
                await channel.SendFileAsync(pixelated);
            } catch {
                try {
                    await channel.SendMessageAsync("I can't send Images here..");
                } catch {
                    // can't even send messages
                }
            }

            try {
                File.Delete(file);
                File.Delete(pixelated);
            } catch (Exception ex) {
                Console.WriteLine($"Couldn't cleanup! ({ex.Message})");
            }
        }
    }
}
