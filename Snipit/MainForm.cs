using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Snipit
{
    public partial class MainForm : Form
    {
        public bool AppRunningState = false;
        public bool useTextExtraction = false;
        public bool useChatGptExtraction = false;
        public bool useLiveExtraction = false;

        public MainForm()
        {

            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mainButton.Text = "Start";
            mainLabel.Visible = false;
            questionPanel.Visible = false;
            responseLabel.Visible = false;
            snipitButton.Visible = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            liveExtraction.Checked = true;
            liveExtraction.Enabled = false;
        }
        private void MainButton_Click(object sender, EventArgs e)
        {
            AppRunningState = !AppRunningState;
            Debug.WriteLine("AppRunningState: " + AppRunningState);
            if (AppRunningState)
            {
                mainButton.Text = "Stop";
                mainLabel.Visible = true;
                snipitButton.Visible = true;
                chatGptExtraction.Enabled = false;
                textExtraction.Enabled = false;
                Program.EnableHelper();
            }
            else
            {
                mainButton.Text = "Start";
                mainLabel.Visible = false;
                responseLabel.Visible = false;
                questionPanel.Visible = false;
                snipitButton.Visible = false;
                chatGptExtraction.Enabled = true;
                textExtraction.Enabled = true;
                Program.DisableHelper();
            }
        }

        private void ChatGptSubmit_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("chatGptSubmit_Click:");
            var question = chatGptQuestion.Text;
            var imagePath = Program.currentImagePath;
            if (string.IsNullOrEmpty(imagePath))
            {
                UpdateResponseLabel("Take a snip first.");
                return;
            }
            question = question.Contains('?') ? question : question + "?";

            chatGptSubmit.Enabled = false;
            UpdateResponseLabel("Loading response...");
            _ = chatGptQuestionEvent(imagePath, question);

        }

        private void Snipit_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Snipit_Click:");
            Program.ShowOverlay();
        }

        private void useTextExtractionChanged(object sender, EventArgs e)
        {
            useTextExtraction = textExtraction.Checked;
            Debug.WriteLine("useTextExtraction: " + useTextExtraction);
        }

        private void useChatGptExtractionChanged(object sender, EventArgs e)
        {
            useChatGptExtraction = chatGptExtraction.Checked;
            Debug.WriteLine("useChatGptExtraction: " + useChatGptExtraction);
        }

        private void useLiveExtractionChanged(object sender, EventArgs e)
        {
            useLiveExtraction = liveExtraction.Checked;
            Debug.WriteLine("useLiveExtraction: " + useLiveExtraction);
        }

        public void UpdatePictureBox(Image image)
        {
            Debug.WriteLine("UpdatePictureBox");
            if (snipitScreenShot.InvokeRequired)
            {
                snipitScreenShot.Invoke(new Action(() => UpdatePictureBox(image)));
                return;
            }
            snipitScreenShot.Image?.Dispose();
            snipitScreenShot.Image = image;
            snipitScreenShot.Invalidate();
            if (useChatGptExtraction)
            {
                questionPanel.Visible = true;
            }
            UpdateResponseLabel("");
        }

        public void UpdateResponseLabel(string text)
        {
            Debug.WriteLine($"UpdateResponseLabel: {text}");
            if (responseLabel.InvokeRequired)
            {
                responseLabel.Invoke(new Action(() => UpdateResponseLabel(text)));
                return;
            }
            responseLabel.Visible = true;
            responseLabel.Text = text;
        }

        private void snipitScreenShotBackgroundChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("snipitScreenShotBackgroundChanged:");
            Program.UpdateSnipitScreenshot();
            snipitScreenShot.BackgroundImage = null;
        }

        private async Task chatGptQuestionEvent(string imagePath, string message)
        {
            Debug.WriteLine($"chatGptQuestionEvent ImagePath:{imagePath} Message:{message}");
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                UpdateResponseLabel("Set the OPENAI_API_KEY environment variable and restart Snipit.");
                chatGptSubmit.Enabled = true;
                return;
            }
            var url = "https://api.openai.com/v1/chat/completions";
            var base64Image = EncodeImageToBase64(imagePath);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var imageUrl = new { url = $"data:image/png;base64,{base64Image}" };
                var contentParts = new object[] { new { type = "text", text = message }, new { type = "image_url", image_url = imageUrl } };
                var body = new { model = "gpt-4o", messages = new[] { new { role = "user", content = contentParts } }, max_tokens = 300 };
                var jsonData = JsonSerializer.Serialize(body);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(url, content);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Response received successfully:");
                        Debug.WriteLine(responseBody);
                        var chatGptResponse = JsonDocument.Parse(responseBody);
                        var contentText = chatGptResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                        UpdateResponseLabel(contentText);
                        SaveJsonResponse(responseBody);
                    }
                    else
                    {
                        Debug.WriteLine($"Error: {response.StatusCode}");
                        Debug.WriteLine(responseBody);
                        using var err = JsonDocument.Parse(responseBody);
                        UpdateResponseLabel($"Error {(int)response.StatusCode}: {err.RootElement.GetProperty("error").GetProperty("message").GetString()}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception: {ex.Message}");
                    UpdateResponseLabel($"Error: {ex.Message}");
                }
                finally
                {
                    chatGptSubmit.Enabled = true;
                }
            }
        }


        private void SaveJsonResponse(string jsonResponse)
        {
            Debug.WriteLine($"SaveJsonResponse");
            try
            {
                var jsonFileName = $"{Program.currentImageTime:yyyyMMdd_HHmmss_fff}.json";
                var jsonFilePath = Path.Combine(Program.gptDirectory, jsonFileName);
                var same_qcount = 2;
                while (File.Exists(jsonFilePath))
                {
                    jsonFileName = $"{Program.currentImageTime:yyyyMMdd_HHmmss_fff}-{same_qcount}.json";
                    jsonFilePath = Path.Combine(Program.gptDirectory, jsonFileName);
                    same_qcount++;
                }
                File.WriteAllText(jsonFilePath, jsonResponse);
                Debug.WriteLine($"JSON response saved to: {jsonFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving JSON response: {ex.Message}");
            }
        }

        private string EncodeImageToBase64(string imagePath)
        {
            Debug.WriteLine($"EncodeImageToBase64");
            try
            {
                var imageBytes = File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error encoding image: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
