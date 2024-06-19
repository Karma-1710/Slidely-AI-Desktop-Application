Imports System.Net.Http
Imports Newtonsoft.Json

Public Class SearchForm
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchEmail As String = txtSearchEmail.Text.Trim()

        If String.IsNullOrEmpty(searchEmail) Then
            MessageBox.Show("Please enter an email to search.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        SearchSubmissionByEmail(searchEmail)
    End Sub

    Private Async Sub SearchSubmissionByEmail(email As String)
        Dim apiUrl As String = $"http://localhost:3000/search?email={email}"
        Using httpClient As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await httpClient.GetAsync(apiUrl)

                If response.IsSuccessStatusCode Then
                    Dim jsonString As String = Await response.Content.ReadAsStringAsync()
                    Dim submission As Submission = JsonConvert.DeserializeObject(Of Submission)(jsonString)

                    DisplaySubmissionDetails(submission)
                ElseIf response.StatusCode = Net.HttpStatusCode.NotFound Then
                    MessageBox.Show("No record found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ClearSubmissionDetails()
                Else
                    MessageBox.Show($"Error searching submission: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub DisplaySubmissionDetails(submission As Submission)
        lblName.Text = submission.name
        lblEmail.Text = submission.email
        lblPhoneNumber.Text = submission.phone
        lblGithubLink.Text = submission.github_link
        lblStopwatchTime.Text = submission.stopwatch_time

    End Sub

    Private Sub ClearSubmissionDetails()
        lblName.Text = ""
        lblEmail.Text = ""
        lblPhoneNumber.Text = ""
        lblGithubLink.Text = ""
        lblStopwatchTime.Text = ""

    End Sub

    Private Sub SearchForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            btnSearch.PerformClick()
        End If
    End Sub
End Class
