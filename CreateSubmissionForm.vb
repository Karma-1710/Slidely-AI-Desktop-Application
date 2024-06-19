Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json

Public Class CreateSubmissionForm
    Private stopwatch As New Stopwatch()
    Private submissionId As String = ""
    Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
        If stopwatch.IsRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        UpdateStopwatchDisplay()
    End Sub
    Private Sub UpdateStopwatchDisplay()
        txtStopwatchTime.Text = stopwatch.Elapsed.ToString("hh\:mm\:ss")
    End Sub
    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Not ValidateInput() Then
            Return
        End If
        Dim submissionData As New With {
            .name = txtName.Text,
            .email = txtEmail.Text,
            .phone = txtPhoneNumber.Text,
            .github_link = txtGithubLink.Text,
            .stopwatch_time = txtStopwatchTime.Text
        }
        Dim jsonData As String = JsonConvert.SerializeObject(submissionData)
        Console.WriteLine(jsonData)

        Dim apiUrl As String = "http://localhost:3000/submit"

        Using httpClient As New HttpClient()
            Try
                Dim content As New StringContent(jsonData, Encoding.UTF8, "application/json")
                Dim response As HttpResponseMessage = Await httpClient.PostAsync(apiUrl, content)

                If response.IsSuccessStatusCode Then
                    MessageBox.Show("Submission successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Close()
                Else
                    MessageBox.Show($"Error submitting form: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Function ValidateInput() As Boolean
        Dim nameRegex As New Regex("^[A-Za-z\s\-']+$")
        If Not nameRegex.IsMatch(txtName.Text) Then
            MessageBox.Show("Please enter a valid name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtName.Focus()
            Return False
        End If

        Dim emailRegex As New Regex("^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
        If Not emailRegex.IsMatch(txtEmail.Text) Then
            MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtEmail.Focus()
            Return False
        End If

        Dim phoneRegex As New Regex("^\d{10}$")
        If Not phoneRegex.IsMatch(txtPhoneNumber.Text) Then
            MessageBox.Show("Please enter a valid 10-digit phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtPhoneNumber.Focus()
            Return False
        End If

        Dim githubRegex As New Regex("^https?://[\w\.]+\.[a-z]{2,6}/?([\w\.-]*)/?$")
        If Not githubRegex.IsMatch(txtGithubLink.Text) Then
            MessageBox.Show("Please enter a valid GitHub link.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtGithubLink.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub CreateSubmissionForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.T Then
            e.SuppressKeyPress = True
            btnToggleStopwatch.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.S Then
            e.SuppressKeyPress = True
            btnSubmit.PerformClick()
        End If
    End Sub
End Class