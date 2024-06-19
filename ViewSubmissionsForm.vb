Imports System.Net.Http
Imports Newtonsoft.Json

Public Class ViewSubmissionsForm
    Private currentIndex As Integer = 0

    Private Async Sub ViewSubmissionsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await FetchSubmission(currentIndex)
    End Sub

    Private Async Function FetchSubmission(index As Integer) As Task
        Dim apiUrl As String = $"http://localhost:3000/read?index={index}"
        Using httpClient As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await httpClient.GetAsync(apiUrl)

                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim submission As SubmissionData = JsonConvert.DeserializeObject(Of SubmissionData)(jsonResponse)

                    txtName.Text = submission.name
                    txtEmail.Text = submission.email
                    txtPhoneNumber.Text = submission.phone
                    txtGithubLink.Text = submission.github_link
                    txtStopwatchTime.Text = submission.stopwatch_time
                ElseIf response.StatusCode = Net.HttpStatusCode.NotFound Then
                    If index < 0 Then
                        MessageBox.Show("This is the first submission.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        currentIndex = 0
                    Else
                        MessageBox.Show("There are no more Submissions..", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        currentIndex -= 1
                    End If
                Else
                    MessageBox.Show($"Error fetching submission: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Function

    Private Class SubmissionData
        Public Property name As String
        Public Property email As String
        Public Property phone As String
        Public Property github_link As String
        Public Property stopwatch_time As String
    End Class

    Private Async Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        If currentIndex > 0 Then
            currentIndex -= 1
            Await FetchSubmission(currentIndex)
        Else
            MessageBox.Show("This is the first submission.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Async Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        currentIndex += 1
        Await FetchSubmission(currentIndex)
    End Sub

    Private Sub ViewSubmissionsForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.P Then
            e.SuppressKeyPress = True
            btnPrevious.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.N Then
            e.SuppressKeyPress = True
            btnNext.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.D Then
            e.SuppressKeyPress = True
            btnDelete.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.E Then
            e.SuppressKeyPress = True
            btnEdit.PerformClick()
        End If
    End Sub

    Private Async Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this submission?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Await DeleteSubmission(currentIndex)
        End If
    End Sub

    Private Async Function DeleteSubmission(index As Integer) As Task
        Dim apiUrl As String = $"http://localhost:3000/delete?index={index}"
        Using httpClient As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await httpClient.DeleteAsync(apiUrl)

                If response.IsSuccessStatusCode Then
                    Me.Close()
                    MessageBox.Show("Submission deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Dim submissions As List(Of SubmissionData) = Await FetchAllSubmissions()

                    ' Check if the deleted index was the last index
                    If submissions.Count = 0 Then
                        MessageBox.Show("No more submissions available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ElseIf index >= submissions.Count Then
                        ' Reset to first index if current index is out of range
                        Await FetchSubmission(0)
                    Else
                        Await FetchSubmission(index)
                    End If
                Else
                    MessageBox.Show($"Error deleting submission: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Function

    Private Async Function FetchAllSubmissions() As Task(Of List(Of SubmissionData))
        Dim apiUrl As String = "http://localhost:3000/readAll"
        Using httpClient As New HttpClient()
            Dim response As HttpResponseMessage = Await httpClient.GetAsync(apiUrl)
            If response.IsSuccessStatusCode Then
                Dim jsonString As String = Await response.Content.ReadAsStringAsync()
                Return JsonConvert.DeserializeObject(Of List(Of SubmissionData))(jsonString)
            Else
                MessageBox.Show($"Error fetching submissions: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return New List(Of SubmissionData)()
            End If
        End Using
    End Function
    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Dim viewForm As New EditForm(currentIndex)
        viewForm.Show()
        Me.Close()
    End Sub
End Class