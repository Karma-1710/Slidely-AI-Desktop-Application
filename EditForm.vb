Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Text

Public Class EditForm
    Private currentIndex As Integer
    Private originalStopwatchTime As String
    Private stopwatch As New Stopwatch()

    Public Sub New(index As Integer)
        InitializeComponent()
        currentIndex = index
        LoadSubmissionData(index)
    End Sub

    Private Async Sub LoadSubmissionData(index As Integer)
        Dim apiUrl As String = $"http://localhost:3000/read?index={index}"
        Using httpClient As New HttpClient()
            Dim response As HttpResponseMessage = Await httpClient.GetAsync(apiUrl)
            If response.IsSuccessStatusCode Then
                Dim jsonString As String = Await response.Content.ReadAsStringAsync()
                Dim submission As Submission = JsonConvert.DeserializeObject(Of Submission)(jsonString)

                ' Fill the form with the existing data
                txtName.Text = submission.name
                txtEmail.Text = submission.email
                txtPhoneNumber.Text = submission.phone
                txtGithubLink.Text = submission.github_link
                txtStopwatchTime.Text = submission.stopwatch_time


            Else
                MessageBox.Show($"Error fetching submission: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Sub

    Private Sub UpdateStopwatchDisplay()
        txtStopwatchTime.Text = stopwatch.Elapsed.ToString("hh\:mm\:ss")
    End Sub

    Private Sub btnEditStopwatch_Click(sender As Object, e As EventArgs) Handles btnEditStopwatch.Click
        If stopwatch.IsRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        UpdateStopwatchDisplay()
    End Sub


    Private Async Sub btnSubmitNew_Click(sender As Object, e As EventArgs) Handles btnSubmitNew.Click
        Dim apiUrl As String = $"http://localhost:3000/update?index={currentIndex}"
        Dim updatedSubmission As New Submission With {
            .name = txtName.Text,
            .email = txtEmail.Text,
            .phone = txtPhoneNumber.Text,
            .github_link = txtGithubLink.Text,
            .stopwatch_time = txtStopwatchTime.Text
        }

        Dim jsonData As String = JsonConvert.SerializeObject(updatedSubmission)
        Using httpClient As New HttpClient()
            Try
                Dim content As New StringContent(jsonData, Encoding.UTF8, "application/json")
                Dim response As HttpResponseMessage = Await httpClient.PutAsync(apiUrl, content)

                If response.IsSuccessStatusCode Then
                    MessageBox.Show("Submission updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Close()
                Else
                    MessageBox.Show($"Error updating submission: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub EditForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.U Then
            e.SuppressKeyPress = True
            btnSubmitNew.PerformClick()
        End If
    End Sub
End Class

Public Class Submission
    Public Property name As String
    Public Property email As String
    Public Property phone As String
    Public Property github_link As String
    Public Property stopwatch_time As String
End Class